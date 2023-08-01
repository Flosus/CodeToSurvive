namespace CodeToSurvive.Lib.Core


module Position =

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
