namespace CodeToSurvive.App.Security

open Microsoft.AspNetCore.Identity

module SecurityModel =

    type AccountRole =
        | User
        | Admin
        
    [<AllowNullLiteral>]
    type ApplicationUser(username, role) =
        inherit IdentityUser(username)
        member val Role: AccountRole = role with get, set
   
    type LoginModel =
        | ActiveLogin of ApplicationUser
        | AnonymousAccess
        | InvalidLogin
    
