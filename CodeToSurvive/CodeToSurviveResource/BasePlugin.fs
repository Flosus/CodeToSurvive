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
        log.LogInformation $"Setup {pluginName}"
        ctx

    let preTickUpdate (ctx: WorldContext) : WorldContext =
        let log = getLog ctx
        log.LogInformation $"PreTickUpdate {pluginName}"
        ctx

    let postTickUpdate (ctx: WorldContext) : WorldContext =
        let log = getLog ctx
        log.LogInformation $"PostTickUpdate {pluginName}"
        ctx

    type BasePlugin() as self =
        inherit Plugin(pluginName, [||])

        do
            self.OnStartup <- Some(onStartup)
            self.PreTickUpdate <- Some(preTickUpdate)
            self.PostTickUpdate <- Some(postTickUpdate)

    let pluginFactory (_: ILoggerFactory) : Plugin =
        let plugin = BasePlugin()
        plugin


    let register () =
        PluginRegistry.registerPlugin pluginFactory
