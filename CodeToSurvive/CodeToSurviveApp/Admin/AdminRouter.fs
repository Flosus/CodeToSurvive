namespace CodeToSurvive.App.Admin

open CodeToSurvive.App.Public.PublicHandler
open Microsoft.AspNetCore.Http

module AdminRouter =
    open Giraffe


    // GET
    let adminRoute = route "/admin" >=> indexHandler

    let adminGetRoutes: (HttpFunc -> HttpContext -> HttpFuncResult) list =
        [ adminRoute ]
    // POST
    let adminPostRoutes: (HttpFunc -> HttpContext -> HttpFuncResult) list =
        [

        ]
