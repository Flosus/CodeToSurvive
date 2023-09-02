namespace CodeToSurvive.Resource

open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Plugin
open CodeToSurvive.Lib.Core.Plugin.PluginApi
open CodeToSurvive.Lib.Core.Tick
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

    let register () =
        let basePlugin = BasePlugin()
        PluginRegistry.registerPlugin basePlugin
