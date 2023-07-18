namespace CodeToSurvive.App.Admin

open CodeToSurvive.App.Admin.AdminHandler

module AdminRouter =
    open Giraffe

    // GET
    let adminRoute = route "" >=> redirectTo false "/secured/private"
    let adminOverviewRoute = route "/overview" >=> adminOverviewHandler

    // POST

    let adminRoutes: HttpHandler =
        let notLoggedIn = setStatusCode 401 >=> redirectTo false "/"

        subRoute
            "/secured/admin"
            (requiresAuthentication notLoggedIn
             >=> choose [ GET >=> choose [ adminRoute; adminOverviewRoute ]; POST >=> choose [ ] ])
