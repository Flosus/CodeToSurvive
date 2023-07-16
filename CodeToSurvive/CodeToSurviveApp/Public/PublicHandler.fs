namespace CodeToSurvive.App.Public

open System
open System.Threading
open System.Threading.Tasks
open CodeToSurvive.App.Security.SecurityModel
open CodeToSurvive.App.Public.PublicViews
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Identity
open CodeToSurvive.App.Public.PublicModels
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.Htmx

module PublicHandler =

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

    let buildPublicModel ctx =
        task {
            let! currentUser = tryGetCurrentUser ctx

            let model =
                { loginModel = currentUser
                  isHtmxRequest = ctx.Request.IsHtmx }

            return model
        }

    let builderModelView (model: PublicModel) (viewFunc: PublicModel -> XmlNode list) httpFunc (ctx: HttpContext) =
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
                let! model = buildPublicModel ctx
                return! builderModelView model indexView next ctx
            }

    let scoreboardHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "scoreboardHandler called"
                let! model = buildPublicModel ctx
                return! builderModelView model scoreboardView next ctx
            }

    let loginHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "loginHandler called"
                let! model = buildPublicModel ctx
                return! builderModelView model loginView next ctx
            }

    let logoutHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "logoutHandler called"
                let! model = buildPublicModel ctx

                let signOutView =
                    signOut "Identity.Application" >=> builderModelView model logoutView

                return! signOutView next ctx
            }

    let notFoundHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("notFoundHandler")
                logger.LogTrace "notFoundHandler called"
                let! model = buildPublicModel ctx
                return! builderModelView model notFoundView next ctx
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
                    let publicModel =
                        { loginModel = LoginModel.InvalidLogin
                          isHtmxRequest = true }

                    return! builderModelView publicModel loginView next ctx
            }

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex: Exception) (logger: ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
