namespace CodeToSurvive.Lib.Core

module World =

    type Chunk = { Name: string; Description: string }

    type WorldMap = { Chunks: ResizeArray<Chunk> }
