namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Security.SecurityModel


module PublicModels =

    type LoginRequest() =
        member val username = "" with get, set
        member val password = "" with get, set


    type PublicModel =
        { loginModel: LoginModel
          isHtmxRequest: bool }
