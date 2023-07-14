namespace CodeToSurvive.App.Public

open System
open CodeToSurvive.App.AuthenticationService
open CodeToSurvive.App.LoginManagement

module PublicModel =
    type LoginModel = ActiveLogin of ActiveLogin | AnonymousAccess | InvalidLogin
    
    let getLogin model: Option<ActiveLogin> =
        match model with
        | AnonymousAccess -> None
        | InvalidLogin -> None
        | ActiveLogin login -> Some login
            

    let DUMMY_ADMIN = { UserId = Guid.Empty; CookieId = Guid.Empty; LastLoginTime = DateTime.Now; Role = AccountRole.Admin }
    let DUMMY_USER = { UserId = Guid.Empty; CookieId = Guid.Empty; LastLoginTime = DateTime.Now; Role = AccountRole.User }

