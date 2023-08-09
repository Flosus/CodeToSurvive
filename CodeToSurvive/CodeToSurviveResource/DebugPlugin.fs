namespace CodeToSurvive.Resource

open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Plugin
open CodeToSurvive.Lib.Core.Plugin.PluginApi
open CodeToSurvive.Lib.Core.Tick
open Microsoft.Extensions.Logging

module DebugPlugin =

    let pluginName = "DebugPlugin"

    let getLog ctx = ctx.CreateLogger pluginName

    let onStartup (ctx: WorldContext) : WorldContext =
        let log = getLog ctx
        log.LogInformation $"Setup ${pluginName}"
        ctx

    let debugPlugin: Plugin =
        { PluginId = pluginName
          Dependencies = [| BasePlugin.pluginName |]
          OnStartup = onStartup }

    let register () =
        PluginRegistry.registerPlugin debugPlugin
