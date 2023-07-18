namespace CodeToSurvive.App.Admin


open CodeToSurvive.App.Private.PrivateViews
open CodeToSurvive.App.Admin.AdminViews
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.Logging

module AdminHandler =
    
    let adminOverviewHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "overviewHandler called"
                let html = adminOverviewView |> htmxContent |> htmlView
                return! html next ctx
            }
