namespace CodeToSurviveLib.CharacterScript.Api

open System
open System.Collections.Generic
open CodeToSurviveLib.Core.Domain
open NLua

module ScriptApi =

    type LoggingApi(character: CharacterState, ctx: WorldContext) =
        member this.think(message: string) =
            character.HandleLogEntry(LogType.Thought, character.Character.Name, DateTime.Now, message)

        member this.thonk(message: string) =
            character.HandleLogEntry(LogType.Thonk, character.Character.Name, DateTime.Now, message)

    type CommunicationApi(character: CharacterState, ctx: WorldContext) =
        member this.say(message: string) =
            character.HandleLogEntry(LogType.Say, character.Character.Name, DateTime.Now, message)

        member this.yell(message: string) =
            character.HandleLogEntry(LogType.Yell, character.Character.Name, DateTime.Now, message)

        member this.whisper(target: string, message: string) =
            character.HandleLogEntry(LogType.Whisper, character.Character.Name, DateTime.Now, message)

        member this.getLastWords() =
            // TODO get this somewhere?!
            [| "Test" |]

    type MemoryApi(character: CharacterState, ctx: WorldContext) =

        let luaTableToDict (luaTable: LuaTable) typeEnsurer : Dictionary<string, Object> =
            let dict = Dictionary<string, Object>()

            luaTable.Keys
            |> Seq.cast
            |> Seq.map (fun k -> (k, typeEnsurer luaTable[k]))
            |> Seq.iter dict.Add

            dict

        let rec ensureType (input: Object) : Object =
            if input :? LuaTable then
                let luaTable = input :?> LuaTable
                luaTableToDict luaTable ensureType
            else
                input

        member this.getKnowledge() = character.Memory.Knowledge

        member this.getMemory() = character.Memory.PlayerMemory

        member this.getMemoryKeys() = character.Memory.PlayerMemory.Keys
        member this.getMemoryValue key = character.Memory.PlayerMemory[key]

        member this.setMemory(key, value: Object) =
            character.Memory.PlayerMemory[key] <- ensureType value

    type WorldApi(character: CharacterState, ctx: WorldContext) =
        member this.getWorld() = ()

    type CharacterApi(character: CharacterState, ctx: WorldContext) =   
        member this.getCharacter() = ()

    type ActionApi(character: CharacterState, ctx: WorldContext) =
        member this.getCurrentAction() = ()
