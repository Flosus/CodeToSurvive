namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.Plugin
open CodeToSurviveLib.Core.Plugin.PluginApi
open CodeToSurviveResource.BasePlugin
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module BasePlugin =

    type BasePlugin() as self =
        inherit Plugin(pluginName, [||])

        do
            self.OnStartup <- Some(OnStartup.onStartup)
            self.PreTickUpdate <- Some(PreTickUpdate.preTickUpdate)
            self.PostTickUpdate <- Some(PostTickUpdate.postTickUpdate)
            // TODO implement the following
            self.GenerateChunk <- Some(ChunkGeneration.generateChunk)
            self.GetSpawnChunk <- Some(ChunkGeneration.getSpawnChunk)
            self.ProgressAction <- None

    let pluginFactory (_: ILoggerFactory) : Plugin =
        let plugin = BasePlugin()
        plugin


    let register () =
        PluginRegistry.registerPlugin pluginFactory
