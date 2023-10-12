namespace CodeToSurviveResource

open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Plugin
open CodeToSurviveLib.Core.Plugin.PluginApi
open Microsoft.Extensions.Logging

module DebugPlugin =

    let pluginName = "DebugPlugin"

    let getLog ctx = ctx.CreateLogger pluginName

    let onStartup (ctx: WorldContext) : WorldContext =
        let log = getLog ctx
        log.LogInformation $"Setup ${pluginName}"
        ctx

    type DebugPlugin() as self =
        inherit Plugin(pluginName, [| BasePlugin.pluginName |])
        do self.OnStartup <- Some(onStartup)

    let pluginFactory (_: ILoggerFactory) : Plugin =
        let plugin = DebugPlugin()
        plugin

    let register () =
        PluginRegistry.registerPlugin pluginFactory
