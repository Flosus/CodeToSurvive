namespace CodeToSurvive.App.Public

open System
open CodeToSurvive.App.AuthenticationService
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

    let tryGetCurrentUser (ctx: HttpContext) =
        let auth = getAuthService ctx
        auth.GetCurrentUser ctx

    let builderModelView (model: LoginModel) (viewFunc: LoginModel -> XmlNode list) httpFunc (ctx: HttpContext) =
        let view = viewFunc model

        let viewRes =
            match ctx.Request.IsHtmx with
            | true -> view |> (div [])
            | false -> view |> layout model

        let viewResult = htmlView viewRes
        viewResult httpFunc ctx

    let indexHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "indexHandler called"
        let model = tryGetCurrentUser ctx
        builderModelView model indexView httpFunc ctx

    let scoreboardHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "scoreboardHandler called"
        let model = tryGetCurrentUser ctx
        builderModelView model scoreboardView httpFunc ctx

    let loginHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "loginHandler called"
        let model = tryGetCurrentUser ctx
        builderModelView model loginView httpFunc ctx

    let logoutHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "logoutHandler called"

        let model = tryGetCurrentUser ctx
        let signOutView = signOut "Cookie" >=> builderModelView model logoutView

        signOutView httpFunc ctx

    let loginRequestHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("loginRequestHandler")
        logger.LogTrace "loginRequestHandler called"

        task {
            let! reqData = ctx.Request.ReadFormAsync()
            let username = reqData["username"]
            let password = reqData["password"]
            let authService = getAuthService ctx
            let result = authService.Login (string username) (string password)

            match result with
            | Some login ->
                let _ = ActiveLogin(login)
                ctx.Response.Cookies.Append(secCookieName, result.Value.CookieId.ToString())
                let redirect = withHxRedirect "/private"
                return! redirect httpFunc ctx
            | None -> return! builderModelView InvalidLogin loginView httpFunc ctx
        }

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
