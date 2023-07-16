namespace CodeToSurvive.Lib.Core

open System.Threading
open CodeToSurvive.Lib.Core.Position

module World =

    exception WorldGenError of string
    let private worldMapLock = obj ()

    type ChunkFeature = string
    type ChunkType = string * ChunkFeature[]

    type Tile =
        { TileType: string
          CanViewOver: bool
          Walkable: bool
          EntitiesAllowed: bool
          Position: TilePosition }

    type Chunk =
        { Name: string
          Description: string
          Location: ChunkPosition
          MainType: ChunkType
          NorthType: ChunkType
          SouthType: ChunkType
          EastType: ChunkType
          WestType: ChunkType
          // Size 64x64
          Tiles: Tile[][] }

    type WorldMap = { Chunks: ResizeArray<Chunk> }

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
        let generator worldMap chunkPosition =
            let chunk = gChunk worldMap chunkPosition
            appendWorldMap worldMap chunk

        generator
