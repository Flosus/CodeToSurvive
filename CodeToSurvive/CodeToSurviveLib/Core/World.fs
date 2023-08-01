namespace CodeToSurvive.Lib.Core

open System.Threading
open CodeToSurvive.Lib.Core.Position

module World =

    exception WorldGenError of string
    let private worldMapLock = obj ()
    type ChunkFeature = string
    type ChunkType = string * ChunkFeature[]

    type Chunk =
        { Name: string
          Description: string
          Location: ChunkPosition
          MainType: ChunkType
          NorthType: ChunkType
          SouthType: ChunkType
          EastType: ChunkType
          WestType: ChunkType }

    type WorldMap =
        { Chunks: ResizeArray<Chunk>
          SpecialChunks: ResizeArray<Chunk> }

    type GenerateChunk = WorldMap -> ChunkPosition -> Chunk
    type UpdateWorldMap = WorldMap -> ChunkPosition -> WorldMap

    let getChunk worldMap chunkPosition =
        let index = worldMap.Chunks.FindIndex(fun cnk -> cnk.Location = chunkPosition)

        match index with
        | -1 -> None
        | _ -> Some(worldMap.Chunks[index])

    let private appendWorldMap (worldMap: WorldMap) (chunk: Chunk) : WorldMap =
        Monitor.Enter worldMapLock
        let existingChunk = getChunk worldMap chunk.Location

        try
            match existingChunk.IsNone with
            | true ->
                printfn $"Duplicate chunk at location {chunk.Location}"
                raise (WorldGenError($"Duplicate chunk at location {chunk.Location}"))
            | false ->
                worldMap.Chunks.Add(chunk)
                worldMap
        finally
            Monitor.Exit worldMapLock

    let getGenerator (gChunk: GenerateChunk) : UpdateWorldMap =
        fun worldMap chunkPosition ->
            let chunk = gChunk worldMap chunkPosition
            appendWorldMap worldMap chunk
