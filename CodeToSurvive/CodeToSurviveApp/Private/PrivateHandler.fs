namespace CodeToSurvive.App.Private

open System
open CodeToSurvive.App.Security.SecurityModel
open CodeToSurvive.App.Public.PublicViews
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Authentication
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

                return! builderModelView model overviewView next ctx
            }
