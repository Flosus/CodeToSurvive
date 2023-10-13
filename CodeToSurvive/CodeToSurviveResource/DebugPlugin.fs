namespace CodeToSurviveResource

open CodeToSurviveLib.Core.Plugin
open CodeToSurviveLib.Core.Plugin.PluginApi
open CodeToSurviveResource.DebugPlugin
open Microsoft.Extensions.Logging

module DebugPlugin =

    type DebugPlugin() as self =
        inherit Plugin(Constants.pluginName, [| BasePlugin.Constants.pluginName |])

        do
            self.OnStartup <- Some(OnStartup.onStartup)
            self.PreTickUpdate <- Some(PreTickUpdate.preTickUpdate)
            self.PostTickUpdate <- Some(PostTickUpdate.postTickUpdate)
            // TODO implement the following
            self.GenerateChunk <- None
            self.GetSpawnChunk <- None
            self.ProgressJob <- None

    let pluginFactory (_: ILoggerFactory) : Plugin =
        let plugin = DebugPlugin()
        plugin

    let register () =
        PluginRegistry.registerPlugin pluginFactory
