namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Private.PrivateHandler
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.Logging

module PrivateRouter =


    let authFailedHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            let logger = ctx.GetLogger("authFailedHandler")
            logger.LogWarning "User is not authenticated"
            redirectTo false "/" next ctx


    // GET
    let privateRoute = route "/private" >=> privateHandler

    // POST


    let privateRoutes: HttpHandler =
        subRoute
            "/secured"
            (requiresAuthentication authFailedHandler
             >=> choose [ GET >=> choose [ privateRoute ]; POST >=> choose [ privateRoute ] ])
