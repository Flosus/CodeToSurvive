namespace CodeToSurvive.Lib.Core


module Position =

    let chunkSize = 64

    
    [<Struct>]
    type TilePosition =
        { X: int
          Y: int }

        static member minusX offset position : TilePosition =
            { position with
                X = position.X - offset }

        static member minusY offset position : TilePosition =
            { position with
                Y = position.Y - offset }

        static member plusX offset position : TilePosition =
            { position with
                X = position.X + offset }

        static member plusY offset position : TilePosition =
            { position with
                Y = position.Y + offset }

    [<Struct>]
    type ChunkPosition =
        { X: int
          Y: int }

        static member minusX offset position : ChunkPosition =
            { position with
                X = position.X - offset }

        static member minusY offset position : ChunkPosition =
            { position with
                Y = position.Y - offset }

        static member plusX offset position : ChunkPosition =
            { position with
                X = position.X + offset }

        static member plusY offset position : ChunkPosition =
            { position with
                Y = position.Y + offset }

    [<Struct>]
    type AbsoluteTilePosition =
        { X: int
          Y: int }

        static member minusX offset position : AbsoluteTilePosition=
            { position with
                X = position.X - offset }

        static member minusY offset position : AbsoluteTilePosition =
            { position with
                Y = position.Y - offset }

        static member plusX offset position : AbsoluteTilePosition=
            { position with
                X = position.X + offset }

        static member plusY offset position : AbsoluteTilePosition =
            { position with
                Y = position.Y + offset }


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
