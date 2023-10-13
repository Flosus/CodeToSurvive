namespace CodeToSurviveResource.DebugPlugin

open CodeToSurviveLib.Core.GameState
open CodeToSurviveResource.DebugPlugin.Constants
open Microsoft.Extensions.Logging

module PreTickUpdate =

    let preTickUpdate (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.PreTickUpdate"
        log.LogTrace $"Did something"
        ctx
