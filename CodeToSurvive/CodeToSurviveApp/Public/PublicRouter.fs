namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Public.PublicHandler
open Microsoft.AspNetCore.Http

module PublicRouter =
    open Giraffe
    // GET
    let indexRoute = route "/" >=> indexHandler
    let loginRoute = route "/login" >=> loginHandler
    let logoutRoute = route "/logout" >=> logoutHandler

    let publicGetRoutesHandler: (HttpFunc -> HttpContext -> HttpFuncResult) list =
        [ indexRoute; loginRoute; logoutRoute ]

    // POST
    let postLoginRoute = route "/login" >=> loginRequestHandler

    let publicPostRoutesHandler: (HttpFunc -> HttpContext -> HttpFuncResult) list =
        [ postLoginRoute ]
