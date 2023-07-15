namespace CodeToSurvive.App

open System
open System.Security.Principal
open Microsoft.Extensions.Logging
open System.Net
open System.Security.Claims
open System.Threading.Tasks
open CodeToSurvive.App.LoginManagement
open CodeToSurvive.Lib.Storage
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Http
open Giraffe

module AuthenticationService =
    type ActiveLogin =
        { UserId: Guid
          CookieId: Guid
          LastLoginTime: DateTime
          Role: AccountRole }
    type LoginModel =
        | ActiveLogin of ActiveLogin
        | AnonymousAccess
        | InvalidLogin

    type LoginState = { activeLogins: ActiveLogin[] }
    let secCookieName = "x-cts-secure"

    let getLogin model : Option<ActiveLogin> =
        match model with
        | AnonymousAccess -> None
        | InvalidLogin -> None
        | ActiveLogin login -> Some login

    type CTSAuthenticationService(serviceCol: IStorage) =
        let storage = serviceCol
        let activeLogins = ResizeArray<ActiveLogin>()
        let lockObj = obj ()

        let getActiveLogins () = lock lockObj (fun () -> activeLogins)

        let getLoginsBy (filter: ActiveLogin -> bool) =
            lock lockObj (fun () -> activeLogins.FindAll filter)

        let removeLogins (shouldRemove: ActiveLogin -> bool) =
            printfn "Removing cookies"
            lock lockObj (fun () -> activeLogins.RemoveAll(fun lI -> shouldRemove lI |> not) |> ignore)
            ()

        let removeLoginsByAge (age: TimeSpan) =
            removeLogins (fun lI -> lI.LastLoginTime.Add(age) < DateTime.Now)

        let removeLoginsByCookie (cookie: Guid) =
            removeLogins (fun lI -> lI.CookieId = cookie)

        let removeLoginsByUser (user: Guid) =
            removeLogins (fun lI -> lI.UserId = user)

        let addOrReplaceLogin (newLogin: ActiveLogin) =
            lock lockObj (fun () ->
                removeLogins (fun lI -> lI.UserId = newLogin.UserId)
                printfn $"NewLoginCookie={newLogin.CookieId}"
                activeLogins.Add newLogin)

        interface IAuthenticationService with
            member this.AuthenticateAsync(context: HttpContext, scheme: string) : Task<AuthenticateResult> =
                task {
                    printfn $"AuthenticateAsync: scheme: {scheme}; context={context}"
                    let ticket = AuthenticationTicket(ClaimsPrincipal(), scheme)
                    return AuthenticateResult.Success(ticket)
                }

            member this.ChallengeAsync(context, a, b) : Task =
                task {
                    let a1 = a
                    let b1 = b
                    printfn "ChallengeAsync called"
                    let secCookie = context.Request.Cookies[secCookieName]
                    let mutable guid = Guid.Empty
                    let isGuid = Guid.TryParse(secCookie, &guid)
                    match isGuid with
                    | false ->
                        context.Response.StatusCode <- int HttpStatusCode.Unauthorized
                        ()
                    | true ->
                        match getActiveLogins () |> Seq.tryFind (fun aL -> aL.CookieId = guid) with
                        | Some _ ->
                            let identity = ClaimsIdentity("cookie")
                            let usr: ClaimsPrincipal = ClaimsPrincipal(identity)
                            context.User <- usr
                            printfn "hmm"
                        | None -> context.Response.StatusCode <- int HttpStatusCode.Unauthorized
                }

            member this.ForbidAsync(context, scheme, properties) : Task =
                task {
                    printfn $"ForbidAsync: scheme: {scheme}; context={context}; properties={properties}"
                    ()
                }

            member this.SignInAsync(context, scheme, principal, properties) : Task =
                task {
                    printfn
                        $"SignInAsync: scheme: {scheme}; context={context}; principal={principal}; properties={properties}"

                    ()
                }

            member this.SignOutAsync(context, scheme, properties) : Task =
                task {
                    printfn $"SignOutAsync: scheme: {scheme}; context={context}; properties={properties}"
                    ()
                }

        member this.Login username password : Option<ActiveLogin> =
            let loginResult = login storage username password

            match loginResult with
            | Error msg ->
                printfn $"Error while logging in {msg}"
                None
            | Success res ->
                let activeLogin =
                    { UserId = res.ID
                      CookieId = Guid.NewGuid()
                      LastLoginTime = DateTime.Now
                      Role = res.Role }

                addOrReplaceLogin activeLogin
                Some(activeLogin)

        member this.GetUserRole userId : AccountRole =
            let usrById =
                getLoginsBy (fun usr -> usr.UserId = userId) |> Seq.findBack (fun _ -> true)

            usrById.Role

        member this.LogoutUser(userId: Guid) = removeLoginsByUser userId

        member this.LogoutSession(cookie: Guid) = removeLoginsByCookie cookie

        member this.GetCurrentUser(ctx: HttpContext) : LoginModel =
            let logger = ctx.GetLogger("GetCurrentUser")
            logger.LogTrace "GetCurrentUser called"
            let secCookie = ctx.Request.Cookies[secCookieName]
            let mutable guid = Guid.Empty
            let isGuid = Guid.TryParse(secCookie, &guid)

            match isGuid with
            | false -> LoginModel.AnonymousAccess
            | true ->
                let activeLogins = getActiveLogins ()
                let loginOpt = activeLogins |> Seq.tryFind (fun aL -> aL.CookieId = guid)

                match loginOpt with
                | None ->
                    logger.LogWarning $"Invalid cookie found: {secCookie}"
                    ctx.Response.Cookies.Delete secCookieName
                    LoginModel.InvalidLogin
                | Some login ->
                    logger.LogTrace "Cookie found"
                    LoginModel.ActiveLogin login
