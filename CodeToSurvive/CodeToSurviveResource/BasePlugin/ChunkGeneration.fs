namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Plugin.Util
open CodeToSurviveLib.Core.World
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module ChunkGeneration =

    let private mapDefinitions = WorldLoader.loadMaps pluginName
    
    let getSpawnChunk (ctx: WorldContext) : Chunk option =
        let log = ctx.CreateLogger $"{pluginName}.ChunkGeneration"
        log.LogInformation $"getSpawnChunk called {mapDefinitions}"
        let silentGladeDefinition = mapDefinitions |> Array.find (fun def -> def.MapId = spawnMapId)
        let newChunk = Chunk(spawnMapId, pluginName, spawnMapId, silentGladeDefinition.Description)
        Some(newChunk)

    let generateChunk (ctx: WorldContext) (char:CharacterState) (chunkId: ChunkId) : Chunk option =
        let log = ctx.CreateLogger $"{pluginName}.ChunkGeneration"
        log.LogInformation $"generateChunk called"
        
        None
