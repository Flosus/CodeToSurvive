namespace CodeToSurviveLib.Core.Plugin

open CodeToSurviveLib.Core.Domain

module PluginApi =

    //___________________________
    // Plugin-Definition
    //___________________________

    type PluginId = string

    type OnStartup = WorldContext -> WorldContext

    /// <summary>
    /// Calls for generating a new chunk.
    /// The list of plugins is reversed and then the first plugin, providing a chunk is used
    /// </summary>
    /// <param name="ctx">The world state.</param>
    /// <param name="chunkId">The expected id of the new chunk</param>
    /// <returns>Some chunk if the plugin was able to generate the chunk</returns>
    type GenerateChunk = WorldContext -> CharacterState -> ChunkId -> Chunk option

    /// Returns the SpawnChunk.
    /// The first plugin that provides a SpawnChunk will get used, but the list of plugin is reversed!
    /// Take care to not duplicate spawn chunks, as this method might be called multiple times. When a chunk with
    /// an existing chunkId already exist it will get dropped.
    type GetSpawnChunk = WorldContext -> Chunk option
    
    type ActionHandler = WorldContext -> CharacterAction -> WorldContext
    type ActionHandlerKey = ActionName * ActionParameter[]

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
        member val ProgressAction: Option<CharacterAction * WorldContext -> WorldContext> = None with get, set
