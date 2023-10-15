namespace CodeToSurviveLib.Core.Player

open System.Collections.Generic
open System.IO
open System.Text
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Script
open CodeToSurviveLib.Util
open Microsoft.Extensions.Logging

module CharacterManager =

    let logEntryToText entry = $"{entry}\n"

    let createCharacter (ctx: WorldContext) name player =
        let log = ctx.CreateLogger "CharacterManager"
        log.LogInformation $"Creating character '{name}' for player '{player}'"

        let getSpawnChunkId () =
            let spawnChunk = WorldUtil.getSpawnChunk ctx
            spawnChunk.Id

        let newCharacter = Character.newCharacter name player getSpawnChunkId
        let storage = ctx.StorageProvider()

        let playerStorage =
            storage.PlayerStorageFolder.CreateSubdirectory $"{newCharacter.Name}"

        let logHandler (entry: LogEntry) =
            let logStorage = playerStorage.CreateSubdirectory "Log"
            let logFile = Path.Join(logStorage.FullName, "player.log")
            let entryTxt = logEntryToText entry
            File.AppendAllText(logFile, entryTxt, Encoding.UTF8)
            ()

        let scriptProvider () =
            let scriptStorage = playerStorage.CreateSubdirectory "Script"

            LuaCharacterScript.getLuaPluginFiles scriptStorage.FullName
            |> Array.map fst
            |> String.concat "\n"

        let newCharacterState =
            { Character = newCharacter
              HandleLogEntry = logHandler
              ScriptProvider = scriptProvider
              Memory = {
                  Knowledge = [|("Entry1", "Data")|]
                  PlayerMemory = Dictionary() 
              } }

        ctx.State.CharacterStates <- ctx.State.CharacterStates |> Array.append [| newCharacterState |]
        log.LogInformation "Character created successfully"
        newCharacterState
