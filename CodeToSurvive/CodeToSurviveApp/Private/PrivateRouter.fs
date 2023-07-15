namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Public.PublicHandler
open CodeToSurvive.App.Private.PrivateHandler
open Microsoft.AspNetCore.Http

module PrivateRouter =
    open Giraffe

    // GET
    let privateRoute = route "/private" >=> privateHandler

    // POST
    let privateRoutes: HttpHandler =
        let notLoggedIn = setStatusCode 401 >=> redirectTo false "/"

        subRoute
            "/secured"
            (requiresAuthentication notLoggedIn
             >=> choose [ GET >=> choose [ privateRoute ]; POST >=> choose [ privateRoute ] ])
