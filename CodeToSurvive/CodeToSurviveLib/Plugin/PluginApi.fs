namespace CodeToSurvive.Lib.Core.Plugin

open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Core.World

module PluginApi =

    //___________________________
    // Plugin-Definition
    //___________________________

    type PluginId = string

    type OnStartup = WorldContext -> WorldContext

    /// Represents a plugin
    type Plugin =
        {
            /// The id of the plugin. Has to be unique
            PluginId: PluginId
            /// An array of pluginIds, which this plugin depends on
            Dependencies: PluginId[]
            OnStartup: OnStartup
        }

    //___________________________
    // Plugin-Api-Definition
    // Used in the registry
    //___________________________


    /// <summary>Calls for generating a new chunk.
    /// The plugin of the source chunk gets called first.</summary>
    /// <param name="ctx">The world state.</param>
    /// <param name="chunk">The source chunk, from which the chunk is reachable.</param>
    /// <param name="chunkId">The expected id of the new chunk</param>
    /// <returns>Some chunk if the plugin was able to generate the chunk</returns>
    type GenerateChunk = WorldState -> Chunk -> ChunkId -> Chunk option

    /// Returns the SpawnChunk.
    /// The last plugin that provides a SpawnChunk will get used.
    type GetSpawnChunk = WorldState -> Chunk

    type BeforeSave = WorldContext -> WorldContext
    type AfterSave = WorldContext -> WorldContext
