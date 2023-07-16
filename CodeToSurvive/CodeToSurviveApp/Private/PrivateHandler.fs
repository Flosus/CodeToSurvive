namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Public.PublicHandler
open CodeToSurvive.App.Private.PrivateViews
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.Logging

module PrivateHandler =

    let privateHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "privateHandler called"
                let! model = buildPublicModel ctx

                return! builderModelView model privateView next ctx
            }

    let overviewHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let logger = ctx.GetLogger("logoutHandler")
                logger.LogTrace "overviewHandler called"
                let html = overviewView |> htmxContent |> htmlView
                return! html next ctx
            }