namespace CodeToSurvive.App

open System
open System.Collections.Generic
open System.Net
open System.Security.Claims
open System.Threading.Tasks
open BCrypt.Net
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Http

module AppSecurity =

    type AccountRole =
        | User
        | Admin

    type Account =
        { ID: Guid
          Username: string
          Role: AccountRole }

    type AccountResult =
        | Success of Account
        | Error of string

    let getUsers (cntx: obj) : Account[] =
        printf "Loading users"
        // TODO load user files
        [||]

    let updatePassword (id: Guid) (newPassword: string) : AccountResult =
        let passwordHash = BCrypt.HashPassword(newPassword)
        // TODO read file
        // TODO replace user entry
        // TODO write file
        Error("Not yet implemented")

    let login (username: string) (password: string) : AccountResult =
        // TODO read file
        // Error("Unavailable")
        // TODO find user in file
        // Error("Not logged in")
        // TODO check password
        let passwordHash = "hashFromFile"

        match BCrypt.Verify(password, passwordHash) with
        | true -> Error("Success")
        // TODO build success
        | false -> Error("Not logged in")


    let createNewUser (username: string) (tempPassword: string) (role: AccountRole) : AccountResult =
        if username.Contains("|") then
            Error("")
        else
            printf $"Creating new user '{username}'"
            let newUserGuid = Guid.NewGuid()
            let passwordHash = BCrypt.HashPassword(tempPassword)
            let fileEntry = $"{newUserGuid}|{passwordHash}|{role}|{username}"
            // TODO read file
            // TODO check for existing username
            // TODO add user entry
            // TODO write file
            let newAccount =
                { ID = newUserGuid
                  Username = username
                  Role = role }

            Success(newAccount)

    type ActiveLogin =
        { UserId: Guid
          CookieId: Guid
          LastLoginTime: DateTime }

    type LoginState = { activeLogins: ActiveLogin[] }

    type CTSAuthenticationService() =
        let mutable activeLogins = ref { activeLogins = [||] }
        let lockObj = obj ()

        let getActiveLogins () =
            lock lockObj (fun () -> activeLogins.Value)

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

        member this.Login username password =

            ()

        member this.LogoutUser(userId: Guid) =

            ()

        member this.LogoutSession(cookie: Guid) =

            ()

