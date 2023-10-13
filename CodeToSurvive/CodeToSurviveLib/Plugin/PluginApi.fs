namespace CodeToSurviveLib.Core.Plugin

open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.World

module PluginApi =
    open CodeToSurviveLib.Core.Action

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
    /// <param name="chunk option">The already generated chunk by an other plugin.</param>
    /// <returns>Some chunk if the plugin was able to generate the chunk</returns>
    type GenerateChunk = WorldState * Chunk * ChunkId * Chunk option -> Chunk option

    /// Returns the SpawnChunk.
    /// The last plugin that provides a SpawnChunk will get used.
    type GetSpawnChunk = CharacterState * WorldState * Chunk option -> Chunk option

    /// Represents a plugin
    type Plugin(pluginId: PluginId, dependencies) =
        /// The id of the plugin. Has to be unique
        member this.PluginId = pluginId
        /// An array of pluginIds, which this plugin depends on
        member this.Dependencies: PluginId[] = dependencies
        member val OnStartup: Option<OnStartup> = None with get, set
        member val GenerateChunk: Option<GenerateChunk> = None with get, set
        member val GetSpawnChunk: Option<GetSpawnChunk> = None with get, set
        member val BeforeSave: Option<WorldContext -> unit> = None with get, set
        member val AfterSave: Option<WorldContext -> unit> = None with get, set
        member val PreTickUpdate: Option<WorldContext -> WorldContext> = None with get, set
        member val PostTickUpdate: Option<WorldContext -> WorldContext> = None with get, set
        member val RunCharacterScripts: Option<WorldContext -> WorldContext> = None with get, set
        member val ProgressJob: Option<CharacterAction * WorldContext -> WorldContext> = None with get, set
