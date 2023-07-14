namespace CodeToSurvive.App

open System
open System.Collections.Generic
open System.Net
open System.Security.Claims
open System.Threading.Tasks
open CodeToSurvive.App.LoginManagement
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Http

module AuthenticationService =

    type ActiveLogin =
        { UserId: Guid
          CookieId: Guid
          LastLoginTime: DateTime
          Role: AccountRole }

    type LoginState = { activeLogins: ActiveLogin[] }

    type CTSAuthenticationService() =
        let mutable activeLogins = ref { activeLogins = [||] }
        let lockObj = obj ()

        let getActiveLogins () =
            lock lockObj (fun () -> activeLogins.Value)

        let getLoginsBy (filter: ActiveLogin -> bool) =
            lock lockObj (fun () -> activeLogins.Value.activeLogins |> Array.filter filter)

        let removeLogins (shouldRemove: ActiveLogin -> bool) =
            lock lockObj (fun () ->
                activeLogins.Value = { activeLogins =
                                         activeLogins.Value.activeLogins
                                         |> Array.filter (fun lI -> shouldRemove lI |> not) })

        let removeLoginsByAge (age: TimeSpan) =
            removeLogins (fun lI -> lI.LastLoginTime.Add(age) < DateTime.Now)

        let removeLoginsByCookie (cookie: Guid) =
            removeLogins (fun lI -> lI.CookieId = cookie)

        let removeLoginsByUser (user: Guid) =
            removeLogins (fun lI -> lI.UserId = user)

        let addOrReplaceLogin (newLogin: ActiveLogin) =
            lock lockObj (fun () ->
                let otherLogins =
                    activeLogins.Value.activeLogins
                    |> Array.filter (fun lI -> lI.UserId = newLogin.UserId)

                let newContent = Array.concat [| otherLogins; [| newLogin |] |]
                activeLogins.Value = { activeLogins = newContent })



        interface IAuthenticationService with
            member this.AuthenticateAsync(context: HttpContext, scheme: string) : Task<AuthenticateResult> =
                task {
                    printfn $"AuthenticateAsync: scheme: {scheme}; context={context}"
                    let ticket = AuthenticationTicket(ClaimsPrincipal(), scheme)
                    return AuthenticateResult.Success(ticket)
                // return AuthenticateResult.Fail("NYI")
                }

            member this.ChallengeAsync(context, scheme, properties) : Task =
                task {
                    printfn "ChallengeAsync called"
                    let secCookie = context.Request.Cookies["x-cts-secure"]
                    let mutable guid = Guid.Empty
                    let isGuid = Guid.TryParse(secCookie, &guid)

                    match isGuid with
                    | false ->
                        context.Response.StatusCode <- int HttpStatusCode.Unauthorized
                        ()
                    | true ->
                        try
                            getActiveLogins().activeLogins
                            |> Array.find (fun aL -> aL.CookieId = guid)
                            |> ignore
                        with :? KeyNotFoundException ->
                            context.Response.StatusCode <- int HttpStatusCode.Unauthorized

                        ()
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

            None

        member this.GetUserRole userId : AccountRole =
            let usrById =
                getLoginsBy (fun usr -> usr.UserId = userId) |> Array.findBack (fun _ -> true)

            usrById.Role

        member this.LogoutUser(userId: Guid) = removeLoginsByUser userId |> ignore

        member this.LogoutSession(cookie: Guid) = removeLoginsByCookie cookie |> ignore
