namespace CodeToSurvive.Lib.Core

module World =
    open System.Collections.Generic

    type ChunkId = string

    type ChunkData = Dictionary<string, obj>

    /// The abstract class of a chunk.
    /// Game Resource Plugins have to implement the specific Chunks
    [<AbstractClass>]
    type Chunk(id: ChunkId, pluginId: string, name: string, description: string) =
        member this.Id = id
        member this.PluginId = pluginId
        member this.Name = name
        member this.Description = description
        member this.ChunkData = ChunkData()

    /// A wrapper type used to contain the world, e.g. all chunks
    type WorldMap = { Chunks: ResizeArray<Chunk> }
