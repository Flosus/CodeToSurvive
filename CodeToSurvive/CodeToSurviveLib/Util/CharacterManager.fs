namespace CodeToSurviveLib.Core.Player

open System.Collections.Generic
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Util
open Microsoft.Extensions.Logging

module CharacterManager =

    let createCharacter (ctx: WorldContext) name player =
        let log = ctx.CreateLogger "CharacterManager"
        log.LogInformation $"Creating character '{name}' for player '{player}'"

        let getSpawnChunkId () =
            let spawnChunk = WorldUtil.getSpawnChunk ctx
            spawnChunk.Id

        let newCharacter = Character.newCharacter name player getSpawnChunkId

        let newCharacterState =
            { Character = newCharacter
              Memory =
                { Knowledge = [||]
                  PlayerMemory = Dictionary() } }

        ctx.State.CharacterStates <- ctx.State.CharacterStates |> Array.append [| newCharacterState |]
        log.LogInformation "Character created successfully"
        newCharacterState
