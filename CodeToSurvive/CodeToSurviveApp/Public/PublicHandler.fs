namespace CodeToSurvive.App.Public

open System
open System.Threading
open System.Threading.Tasks
open CodeToSurvive.App.Security.SecurityModel
open CodeToSurvive.App.Public.PublicViews
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Identity
open Microsoft.Extensions.Logging

module PublicHandler =
    open Giraffe
    open Giraffe.Htmx

    let tryGetCurrentUser (ctx: HttpContext) =
        task {
            let auth = ctx.User

            let loginModel =
                match auth.Identity.IsAuthenticated with
                | true ->
                    task {
                        let usrManager = ctx.GetService<IUserPasswordStore<ApplicationUser>>()
                        let! user = usrManager.FindByNameAsync(auth.Identity.Name, CancellationToken())
                        return LoginModel.ActiveLogin user
                    }
                | false -> Task.FromResult(AnonymousAccess)

            return! loginModel
        }

    let builderModelView (model: LoginModel) (viewFunc: LoginModel -> XmlNode list) httpFunc (ctx: HttpContext) =
        let view = viewFunc model

        let viewRes =
            match ctx.Request.IsHtmx with
            | true -> view |> (div [])
            | false -> view |> layout model

        let viewResult = htmlView viewRes
        viewResult httpFunc ctx

    let indexHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "indexHandler called"
                let! currentUser = tryGetCurrentUser ctx
                let model = currentUser
                return! builderModelView model indexView next ctx
            }

    let scoreboardHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "scoreboardHandler called"
                let! currentUser = tryGetCurrentUser ctx
                let model = currentUser
                return! builderModelView model scoreboardView next ctx
            }

    let loginHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "loginHandler called"
                let! currentUser = tryGetCurrentUser ctx
                let model = currentUser
                return! builderModelView model loginView next ctx
            }

    let logoutHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "logoutHandler called"
                let! currentUser = tryGetCurrentUser ctx

                let signOutView =
                    signOut "Identity.Application" >=> builderModelView currentUser logoutView

                return! signOutView next ctx
            }

    let loginRequestHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("loginRequestHandler")
                logger.LogTrace "loginRequestHandler called"
                let! reqData = ctx.Request.ReadFormAsync()
                let username = reqData["username"]
                let password = reqData["password"]
                let signInManager = ctx.GetService<SignInManager<ApplicationUser>>()

                let! signInResult =
                    signInManager
                    |> fun manager -> manager.PasswordSignInAsync(username, password, true, false)

                if signInResult = SignInResult.Success then
                    let redirect = withHxRedirect "/secured/private"
                    return! redirect next ctx
                else
                    return! builderModelView LoginModel.InvalidLogin loginView next ctx
            }

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
