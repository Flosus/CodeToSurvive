namespace CodeToSurvive.App.Public

open System
open CodeToSurvive.App.AuthenticationService
open CodeToSurvive.App.Public.PublicModel
open CodeToSurvive.App.Public.PublicViews
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

module PublicHandler =
    open Giraffe
    open Giraffe.Htmx

    let getAuthService (ctx: HttpContext) =
        ctx.GetService<IAuthenticationService>() :?> CTSAuthenticationService

    let builderModelView (model:LoginModel) (viewFunc:LoginModel -> XmlNode list) httpFunc (ctx: HttpContext) =
        let view = viewFunc model
        let viewRes = match ctx.Request.IsHtmx with
                        | true  -> view |> (div [])
                        | false -> view |> layout model
        let viewResult = htmlView viewRes
        viewResult httpFunc ctx

    let indexHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "indexHandler called"
        let model = LoginModel.AnonymousAccess
        builderModelView model indexView httpFunc ctx

    let scoreboardHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "scoreboardHandler called"
        let model = LoginModel.AnonymousAccess
        builderModelView model scoreboardView httpFunc ctx

    let loginHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "loginHandler called"
        let model = LoginModel.AnonymousAccess
        builderModelView model loginView httpFunc ctx

    let logoutHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "logoutHandler called"
        let signOutView = signOut "Cookie" >=> builderModelView LoginModel.AnonymousAccess logoutView 
        signOutView httpFunc ctx
      
    let loginRequestHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("loginRequestHandler")
        logger.LogTrace "loginRequestHandler called"
        task {
            let! reqData = ctx.Request.ReadFormAsync()
            let username = reqData["username"]
            let password = reqData["password"]
            let authService = getAuthService ctx
            let result = authService.Login username password
            // TODO replace this stuff?!?
            let loginModel = if result.IsSome then ActiveLogin(DUMMY_USER) else InvalidLogin
            return! builderModelView loginModel loginView httpFunc ctx
        }

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
