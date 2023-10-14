namespace CodeToSurviveLib.Core.Player

open System.IO
open System.Text
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Util
open Microsoft.Extensions.Logging

module CharacterManager =
    
    let logEntryToText entry =
        "${entry}"

    let createCharacter (ctx: WorldContext) name player =
        let log = ctx.CreateLogger "CharacterManager"
        log.LogInformation $"Creating character '{name} for player '{player}'"

        let getSpawnChunkId () =
            let spawnChunk = WorldUtil.getSpawnChunk ctx
            spawnChunk.Id

        let newCharacter = Character.newCharacter name player getSpawnChunkId
        let storage = ctx.StorageProvider ()
        let playerStorage = storage.PlayerStorageFolder.CreateSubdirectory $"{newCharacter.Id}" 

        let logHandler (entry: LogEntry) = 
            let logStorage = playerStorage.CreateSubdirectory $"Log"
            let logFile = Path.Join (logStorage.FullName, "player.log")
            let entryTxt = logEntryToText entry
            File.AppendAllText (logFile, entryTxt, Encoding.UTF8 )
            ()
        let scriptProvider () =
            let scriptStorage = playerStorage.CreateSubdirectory $"Script"
            // TODO find lua file(s?)
            [|""|]

        let newCharacterState =
            { Character = newCharacter
              HandleLogEntry = logHandler
              ScriptProvider = scriptProvider }

        ctx.State.CharacterStates <- ctx.State.CharacterStates |> Array.append [| newCharacterState |]
        log.LogInformation "Character created successfully"
        newCharacterState
