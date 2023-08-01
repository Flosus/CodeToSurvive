namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Position
open CodeToSurvive.Lib.Core.World

module WorldGen =

    // Maybe externalise these or provide an easy option to add more?

    let roadFeature = ChunkFeature "Road"
    let riverFeature = ChunkFeature "River"
    let wallFeature = ChunkFeature "Wall"
    let bridgeFeature = ChunkFeature "Bridge"

    let plainsChunkName = "Plains"
    let swampChunkName = "Swamp"
    let forestChunkName = "Forest"
    let waterChunkName = "Water"

    let plainsType = ChunkType(plainsChunkName, [||])
    let plainsRiverType = ChunkType(plainsChunkName, [| riverFeature |])
    let plainsRoadType = ChunkType(plainsChunkName, [| roadFeature |])
    let plainsWallType = ChunkType(plainsChunkName, [| wallFeature |])
    let swampType = ChunkType(swampChunkName, [||])
    let swampRiverType = ChunkType(swampChunkName, [| riverFeature |])
    let swampRoadType = ChunkType(swampChunkName, [| roadFeature |])
    let forestType = ChunkType(forestChunkName, [||])
    let forestRiverType = ChunkType(forestChunkName, [| riverFeature |])
    let forestRoadType = ChunkType(forestChunkName, [| roadFeature |])
    let waterType = ChunkType(waterChunkName, [||])
    let waterBridgeType = ChunkType(waterChunkName, [| bridgeFeature |])

    type ChunkTemplate =
        { Name: string
          Description: string
          MainType: ChunkType
          NorthType: ChunkType
          SouthType: ChunkType
          EastType: ChunkType
          WestType: ChunkType }

    let randomNumberGen = Random()

    let createChunkFromTemplate position template =
        { Name = template.Name
          Description = template.Description
          Location = position
          MainType = template.MainType
          NorthType = template.NorthType
          SouthType = template.SouthType
          EastType = template.EastType
          WestType = template.WestType }

    let generateChunk isAllowedAdjacent getAllTemplates map position =
        let northChunk = position |> ChunkPosition.plusY 1 |> getChunk map
        let southChunk = position |> ChunkPosition.plusY 1 |> getChunk map
        let westChunk = position |> ChunkPosition.plusX -1 |> getChunk map
        let eastChunk = position |> ChunkPosition.plusX 1 |> getChunk map

        let possibleTemplates =
            getAllTemplates ()
            |> Array.filter (fun t -> northChunk.IsNone || isAllowedAdjacent t.NorthType northChunk.Value.SouthType)
            |> Array.filter (fun t -> southChunk.IsNone || isAllowedAdjacent t.SouthType southChunk.Value.NorthType)
            |> Array.filter (fun t -> westChunk.IsNone || isAllowedAdjacent t.EastType westChunk.Value.WestType)
            |> Array.filter (fun t -> eastChunk.IsNone || isAllowedAdjacent t.WestType eastChunk.Value.EastType)

        let possibleTemplatesLength = possibleTemplates.Length

        match possibleTemplatesLength with
        // TODO add more debug log
        | 0 -> raise (WorldGenError("Missing template"))
        | _ ->
            let randomIndex = randomNumberGen.Next(0, possibleTemplatesLength - 1)
            let template = possibleTemplates[randomIndex]
            createChunkFromTemplate position template
