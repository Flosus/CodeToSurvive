namespace CodeToSurviveResource.BasePlugin

open System
open CodeToSurviveLib.Core.Domain
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module PostTickUpdate =

    let postTickUpdate (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.PostTickUpdate"
        log.LogTrace "Did something"

        ctx
