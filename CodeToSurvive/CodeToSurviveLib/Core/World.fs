namespace CodeToSurviveLib.Core

module World =
    open System.Collections.Generic

    type ChunkId = string

    type ChunkState = Dictionary<string, obj>

    type Action =
        { ActionId: string
          Description: string
          JobName: string
          JobHandler: string
          CheckHandler: string
          HandlerParameter: Dictionary<string, obj> }

    type TransitionDefinition =
        { TargetMapId: string
          Description: string
          DescriptionHandler: string
          Check: string }

    type POI =
        { Name: string
          Description: string
          Actions: Action[] }

    // TODO create real function references
    type Trigger =
        { mutable OnMapEnter: string
          mutable OnMapExit: string
          mutable OnMapStay: string
          mutable OnMapSave: string
          mutable OnMapLoad: string
          mutable OnMapUnload: string }

    /// The abstract class of a chunk.
    /// Game Resource Plugins have to implement the specific Chunks
    type Chunk(id: ChunkId, pluginId: string, name: string, description: string) =
        member this.Id = id
        member this.PluginId = pluginId
        member this.Name = name
        member this.Description = description
        // member val Instanced: bool = false with get, set
        // member val Persistent: bool = false with get, set
        // member val PersistencePool: string option = None with get, set
        // member val Transitions: TransitionDefinition[] = [||] with get, set
        // // member val   Trigger: Trigger with get, set
        // member val POIs: POI[] = [||] with get, set

        member this.State = ChunkState()

    /// A wrapper type used to contain the world, e.g. all chunks
    type WorldMap = { Chunks: ResizeArray<Chunk> }
