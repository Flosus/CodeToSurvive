namespace CodeToSurviveApp.Security

open BCrypt.Net
open CodeToSurvive.App.Security.SecurityModel
open Microsoft.AspNetCore.Identity

type PasswordHasher() =
    interface IPasswordHasher<ApplicationUser> with
        member this.HashPassword(_, password) = BCrypt.HashPassword password

        member this.VerifyHashedPassword(_, hashedPassword, providedPassword) =
            let result = BCrypt.Verify(providedPassword, hashedPassword)

            match result with
            | true -> PasswordVerificationResult.Success
            | false -> PasswordVerificationResult.Failed
