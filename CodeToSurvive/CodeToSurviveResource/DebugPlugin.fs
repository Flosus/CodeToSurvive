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


    type DebugPlugin() as self =
        inherit Plugin(pluginName, [| BasePlugin.pluginName |])
        do self.OnStartup <- Some(onStartup)

    let pluginFactory (loggerFact: ILoggerFactory): Plugin = 
        let plugin = DebugPlugin()
        plugin

    let register () =
        let debugPlug = DebugPlugin()
        PluginRegistry.registerPlugin pluginFactory
