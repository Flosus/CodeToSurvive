namespace CodeToSurvive.App.Admin

open CodeToSurvive.App.Public.PublicHandler
open Microsoft.AspNetCore.Http

module AdminRouter =
    open Giraffe

    // GET
    let adminRoute = route "/secured/admin" >=> indexHandler

    // POST

    let adminRoutes: HttpHandler =
        let notLoggedIn = setStatusCode 401 >=> redirectTo false "/" 
        subRoute
            "/admin"
            (requiresAuthentication notLoggedIn
             >=> choose [ GET >=> choose [ adminRoute ]; POST >=> choose [adminRoute] ])
