namespace CodeToSurvive.Lib.Core.Plugin

open System.Collections.Generic
open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Plugin.PluginApi
open CodeToSurvive.Lib.Core.World

module WorldGenRegistry =

    //___________________________
    // chunk generation
    //___________________________

    exception NoChunkGeneratorFound of string

    let worldGenRegistry = Dictionary<PluginId, GenerateChunk>()

    let registerGenerateChunk plugin generateFunc =
        worldGenRegistry.Add(plugin, generateFunc)

    let rec private _getChunkGenerator (plugins: Plugin[]) ctx chunk chunkId : Chunk =
        let currentPlugin = plugins[0].PluginId

        let possibleChunk =
            if worldGenRegistry.ContainsKey(currentPlugin) then
                worldGenRegistry.Item currentPlugin ctx chunk chunkId
            else
                None


        match possibleChunk, plugins.Length with
        // TODO default chunk?
        | None, 1 -> raise (NoChunkGeneratorFound(chunkId))
        | Some(generateChunk), _ -> generateChunk
        | None, _ -> _getChunkGenerator plugins[1..] ctx chunk chunkId

    let internal getChunkGenerator ctx chunk chunkId =
        let plugins = PluginRegistry.getPlugins () |> Array.rev
        let generatedChunk = _getChunkGenerator plugins ctx chunk chunkId
        generatedChunk

    //___________________________
    // Default chunk
    //___________________________

    /// Set the spawn chunk generator
    let mutable spawnChunkGenerator: GetSpawnChunk option = None

    let getSpawnChunk (state: WorldState) =
        match spawnChunkGenerator with
        | Some(gen) -> gen state
        | None -> raise (NoChunkGeneratorFound("SpawnChunk"))
