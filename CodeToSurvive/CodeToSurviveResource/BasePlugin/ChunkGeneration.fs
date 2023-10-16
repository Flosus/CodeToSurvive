namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin.Util
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module ChunkGeneration =

    let private mapDefinitions = WorldLoader.loadMaps pluginName

    let getSpawnChunk (ctx: WorldContext) : Chunk option =
        let log = ctx.CreateLogger $"{pluginName}.ChunkGeneration"
        log.LogInformation "getSpawnChunk called"

        let silentGladeDefinition =
            mapDefinitions |> Array.find (fun def -> def.MapId = spawnMapId)

        let newChunk =
            Chunk(spawnMapId, pluginName, spawnMapId, silentGladeDefinition.Description)

        Some(newChunk)

    let generateChunk (ctx: WorldContext) (_: CharacterState) (_: ChunkId) : Chunk option =
        let log = ctx.CreateLogger $"{pluginName}.ChunkGeneration"
        log.LogInformation "generateChunk called"

        None
