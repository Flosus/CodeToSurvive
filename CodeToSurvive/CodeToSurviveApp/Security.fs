namespace CodeToSurvive.App

open System
open System.Net
open System.Security.Claims
open System.Threading.Tasks
open BCrypt.Net
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Http

module AppSecurity =

    type CTSAuthenticationService(iServiceProvider: IServiceProvider) =
        let _iServiceProvider = iServiceProvider
        interface IAuthenticationService with
            member this.AuthenticateAsync(context: HttpContext, scheme: string) : Task<AuthenticateResult> =
                task {
                    printfn $"AuthenticateAsync: scheme: {scheme}; context={context}"
                    return AuthenticateResult.Fail("NYI")
                }


            member this.ChallengeAsync(context, scheme, properties) : Task =
                task {
                    let secCookie = context.Request.Cookies["x-cts-secure"]
                    printfn $"ChallengeAsync: scheme: {scheme}; context={context}; properties={properties}"
                    printfn $"cookie='{secCookie}'"
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
                    printfn $"SignInAsync: scheme: {scheme}; context={context}; principal={principal}; properties={properties}"
                    ()
                }

            member this.SignOutAsync(context, scheme, properties) : Task =
                task {
                    printfn $"SignOutAsync: scheme: {scheme}; context={context}; properties={properties}"
                    ()
                }

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
