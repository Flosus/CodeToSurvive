namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Public.PublicHandler
open Microsoft.AspNetCore.Http

module PublicRouter =
    open Giraffe
    // GET
    let indexRoute = route "/" >=> indexHandler
    let scoreboardRoute = route "/scoreboard" >=> scoreboardHandler
    let loginRoute = route "/login" >=> loginHandler
    let logoutRoute = route "/logout" >=> logoutHandler

    // POST
    let postLoginRoute = route "/login" >=> loginRequestHandler

    let publicRoutes: HttpHandler =
        choose
            [ GET >=> choose [ indexRoute; scoreboardRoute; loginRoute; logoutRoute ]
              POST >=> choose [ postLoginRoute ] ]
