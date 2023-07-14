namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Public.PublicHandler
open Microsoft.AspNetCore.Http

module PrivateRouter =
    open Giraffe

    // GET
    let playRoute = route "/private" >=> indexHandler

    let privateGetRoutes: (HttpFunc -> HttpContext -> HttpFuncResult) list =
        [ playRoute ]
    // POST
    let privatePostRoutes: (HttpFunc -> HttpContext -> HttpFuncResult) list =
        [

        ]
