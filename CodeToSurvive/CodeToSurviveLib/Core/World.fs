namespace CodeToSurvive.Lib.Core

open System

module World =
    let chunkSize = 64

    type WorldEntity = { entityName: string }

    [<Struct>]
    type TilePosition = { X: int; Y: int }

    [<Struct>]
    type ChunkPosition = { X: int; Y: int }

    [<Struct>]
    type AbsoluteTilePosition = { X: int; Y: int }

    type Tile =
        { tileType: string
          canViewOver: bool
          walkable: bool
          entity: WorldEntity[]
          position: TilePosition }

    type Chunk =
        { Name: String
          Location: ChunkPosition
          Description: String
          MainType: String
          NorthType: String
          SouthType: String
          EastType: String
          WestType: String
          // Size 64x64
          Tiles: Tile[][] }

    (*
    negPos | posPos
    _______________
    negNeg | posNeg
    *)
    type WorldMap =
        { posPosChunks: Chunk[][]
          posNegChunks: Chunk[][]
          negPosChunks: Chunk[][]
          negNegChunks: Chunk[][] }

    type GenerateTiles = Chunk -> Chunk
    type GenerateChunk = WorldMap -> ChunkPosition -> Chunk

    let private getWorldPart worldMap chunkPosition =
        match chunkPosition with
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } when x >= 0 && y >= 0 -> worldMap.posPosChunks
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } when x >= 0 && y < 0 -> worldMap.posNegChunks
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } when x < 0 && y >= 0 -> worldMap.negPosChunks
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } -> worldMap.negNegChunks

    let private appendWorldMap (worldMap: WorldMap) (chunkPosition: ChunkPosition) (chunk: Chunk) : WorldMap =
        let appendMap (part: Chunk[][]) =
            let xAbs = abs chunkPosition.X
            let yAbs = abs chunkPosition.Y
            part

        match chunkPosition with
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } when x >= 0 && y >= 0 ->
            { worldMap with
                posPosChunks = appendMap worldMap.posPosChunks }
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } when x >= 0 && y < 0 ->
            { worldMap with
                posNegChunks = appendMap worldMap.posNegChunks }
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } when x < 0 && y >= 0 ->
            { worldMap with
                negPosChunks = appendMap worldMap.negPosChunks }
        | { ChunkPosition.X = x
            ChunkPosition.Y = y } ->
            { worldMap with
                negNegChunks = appendMap worldMap.negNegChunks }

    let private tryGetChunk x y (chunkMap: Chunk[][]) =
        if chunkMap.Length >= x then None
        else if chunkMap[x].Length >= y then None
        else Some(chunkMap[x][y])

    let getGenerator (gChunk: GenerateChunk) (gTiles: GenerateTiles) : WorldMap -> ChunkPosition -> WorldMap =
        let generator worldMap chunkPosition =
            let chunk = gChunk worldMap chunkPosition
            let chunkWithTiles = gTiles chunk
            appendWorldMap worldMap chunkPosition chunkWithTiles

        generator

    let getChunk worldMap chunkPosition =
        let worldPart = getWorldPart worldMap chunkPosition
        tryGetChunk chunkPosition.X chunkPosition.Y worldPart

    let getChunkPosition (tilePosition: AbsoluteTilePosition) : ChunkPosition =
        let chunkX = tilePosition.X / chunkSize
        let chunkY = tilePosition.Y / chunkSize
        { X = chunkX; Y = chunkY }

    let getPositionInTile (tilePosition: AbsoluteTilePosition) (chunkPosition: ChunkPosition) : TilePosition =
        let tileX = (abs tilePosition.X) - chunkPosition.X * chunkSize
        let tileY = (abs tilePosition.Y) - chunkPosition.Y * chunkSize
        { X = tileX; Y = tileY }

    let getAbsolutePosition (tilePosition: TilePosition) (chunkPosition: ChunkPosition) : AbsoluteTilePosition =
        let absoluteX = tilePosition.X * chunkPosition.X
        let absoluteY = tilePosition.Y * chunkPosition.Y
        { X = absoluteX; Y = absoluteY }
