namespace CodeToSurviveResource

open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Plugin
open CodeToSurviveLib.Core.Plugin.PluginApi
open Microsoft.Extensions.Logging

module BasePlugin =

    let pluginName = "BasePlugin"
    let getLog ctx = ctx.CreateLogger pluginName

    let onStartup (ctx: WorldContext) : WorldContext =
        let log = getLog ctx
        log.LogInformation $"Setup ${pluginName}"
        ctx

    type BasePlugin() as self =
        inherit Plugin(pluginName, [||])
        do self.OnStartup <- Some(onStartup)

    let pluginFactory (_: ILoggerFactory) : Plugin =
        let plugin = BasePlugin()
        plugin


    let register () =
        PluginRegistry.registerPlugin pluginFactory
