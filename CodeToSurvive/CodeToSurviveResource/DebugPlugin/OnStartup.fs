namespace CodeToSurviveResource.DebugPlugin

open CodeToSurviveLib.Core.GameState
open CodeToSurviveResource.DebugPlugin.Constants
open Microsoft.Extensions.Logging

module OnStartup =

    let onStartup (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.OnStartup"
        log.LogInformation $"Did something"
        ctx
