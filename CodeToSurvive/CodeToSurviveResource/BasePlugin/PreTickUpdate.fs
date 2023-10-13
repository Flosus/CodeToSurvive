namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.GameState
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module PreTickUpdate =

    let preTickUpdate (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.PreTickUpdate"
        log.LogTrace $"Did something"
        ctx
