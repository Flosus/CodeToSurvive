namespace CodeToSurvive.Lib.DevImpl

open CodeToSurvive.Lib.Core.Position
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.Core.WorldGen

module DevWorldGen =

    // ----------------------------------------//
    // DEFAULT IMPLEMENTATIONS FOR DEVELOPMENT //
    // ----------------------------------------//
    let private checkDefaultAllowed
        (exclusiveFeatures: ChunkFeature[])
        (chunkType1: ChunkType)
        (chunkType2: ChunkType)
        =
        let typeName1, features1 = chunkType1
        let typeName2, features2 = chunkType2
        // check type
        let isSameType = typeName1 = typeName2
        // Check feature
        // TODO remove exclusive features before comparison
        let feature1Set = Set.ofArray features1
        let feature2Set = Set.ofArray features2
        let intersection = Set.intersect feature1Set feature2Set
        let isMatchingFeature = intersection = feature1Set && intersection = feature2Set

        let isExclusiveOrElse contains =
            let contains1 = contains features1
            let contains2 = contains features2

            not contains1 && not contains2 && isMatchingFeature
            || not contains1 && contains2
            || contains1 && not contains2

        let notMatching =
            exclusiveFeatures
            |> Array.filter (fun exc -> Array.contains exc |> isExclusiveOrElse |> not)

        isSameType && notMatching.Length = 0

    let private defaultExclusiveFeatures = [| wallFeature |]

    let private isAllowedDefault = checkDefaultAllowed defaultExclusiveFeatures

    // TODO add some default templates
    let private getTemplates () : ChunkTemplate[] =
        [| { Name = "Plains"
             Description = "Empty Plains"
             MainType = plainsType
             NorthType = plainsType
             SouthType = plainsType
             EastType = plainsType
             WestType = plainsType }

           |]
    // Dev impl
    let generateChunkDefault = generateChunk isAllowedDefault getTemplates
