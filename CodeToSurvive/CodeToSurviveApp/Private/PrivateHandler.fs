namespace CodeToSurvive.App.Private

open System
open CodeToSurvive.App.AuthenticationService
open CodeToSurvive.App.Public.PublicViews
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Authentication
open CodeToSurvive.App.Public.PublicHandler
open CodeToSurvive.App.Private.PrivateViews
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.Logging

module PrivateHandler =

    let privateHandler httpFunc (ctx: HttpContext) =
        let logger = ctx.GetLogger("logoutHandler")
        logger.LogTrace "privateHandler called"

        let model = tryGetCurrentUser ctx
        
        builderModelView model overviewView httpFunc ctx
