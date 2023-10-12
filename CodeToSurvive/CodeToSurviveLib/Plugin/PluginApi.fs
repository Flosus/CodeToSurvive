namespace CodeToSurviveLib.Core.Plugin

open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.World

module PluginApi =

    //___________________________
    // Plugin-Definition
    //___________________________

    type PluginId = string

    type OnStartup = WorldContext -> WorldContext

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

    /// Represents a plugin
    type Plugin(pluginId: PluginId, dependencies) =
        /// The id of the plugin. Has to be unique
        member this.PluginId = pluginId
        /// An array of pluginIds, which this plugin depends on
        member this.Dependencies: PluginId[] = dependencies
        member val OnStartup: Option<OnStartup> = None with get, set
        member val GenerateChunk: Option<GenerateChunk> = None with get, set
        member val GetSpawnChunk: Option<GetSpawnChunk> = None with get, set
        member val BeforeSave: Option<BeforeSave> = None with get, set
        member val AfterSave: Option<AfterSave> = None with get, set
