namespace CodeToSurvive.App.Public

open System
open CodeToSurvive.App.AuthenticationService
open CodeToSurvive.App.LoginManagement

module PublicModel =
    
    type LoginRequest() = 
        member val username = "" with get, set
        member val password = "" with get, set
    