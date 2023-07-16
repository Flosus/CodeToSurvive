namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Public.PublicHandler

module PublicRouter =
    open Giraffe
    // GET
    let indexRoute = route "/" >=> indexHandler
    let scoreboardRoute = route "/scoreboard" >=> scoreboardHandler
    let loginRoute = route "/login" >=> loginHandler
    let logoutRoute = route "/logout" >=> logoutHandler
    let notFoundRoute = route "/404" >=> notFoundHandler

    // POST
    let postLoginRoute = route "/login" >=> loginRequestHandler

    let publicRoutes: HttpHandler =
        choose
            [ GET
              >=> choose [ indexRoute; scoreboardRoute; loginRoute; logoutRoute; notFoundRoute ]
              POST >=> choose [ postLoginRoute ] ]
