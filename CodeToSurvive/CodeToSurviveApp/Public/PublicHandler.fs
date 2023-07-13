namespace CodeToSurvive.App.Public

open System
open CodeToSurvive.App.AuthenticationService
open CodeToSurvive.App.Public.PublicModel
open CodeToSurvive.App.Public.PublicViews
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

module PublicHandler =
    open Giraffe

    let indexHandler: HttpFunc -> HttpContext -> HttpFuncResult =
        let model = { ID = Guid.Empty; IsLoggedIn = false }
        let view = index model
        htmlView view

    let loginHandler: HttpFunc -> HttpContext -> HttpFuncResult =
        let model = { ID = Guid.Empty; IsLoggedIn = false }
        let view = loginView model
        htmlView view

    let logoutHandler httpFunc (httpContext: HttpContext) =
        let sO = signOut "Cookie" >=> redirectTo false "/"
        sO httpFunc httpContext

    let getAuthService (httpContext: HttpContext) =
        httpContext.GetService<IAuthenticationService>() :?> CTSAuthenticationService


    let loginRequestHandler httpFunc (httpContext: HttpContext) =
        task {
            let! reqData = httpContext.Request.ReadFormAsync()
            let username = reqData["username"]
            let password = reqData["password"]
            let authService = getAuthService httpContext
            authService.Login username password
            // TODO replace this stuff?!?
            let sSC = setStatusCode 404 >=> text "Not Found"
            return! sSC httpFunc httpContext
        }

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
