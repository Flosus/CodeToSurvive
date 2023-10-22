namespace CodeToSurviveLib.CharacterScript.Api

open System
open CodeToSurviveLib.CharacterScript
open CodeToSurviveLib.Core.Domain

module ScriptApi =

    type LoggingApi(character: CharacterState, ctx: WorldContext) =
        member this.think(message: string) =
            let entry = (LogType.Thought, character.Character.Name, DateTime.Now, message)
            ctx.HandleLogEntry character entry

        member this.thonk(message: string) =
            let entry = (LogType.Thonk, character.Character.Name, DateTime.Now, message)
            ctx.HandleLogEntry character entry

    type CommunicationApi(character: CharacterState, ctx: WorldContext) =
        member this.say(message: string) =
            let entry = (LogType.Say, character.Character.Name, DateTime.Now, message)
            ctx.HandleLogEntry character entry

        member this.yell(message: string) =
            let entry = (LogType.Yell, character.Character.Name, DateTime.Now, message)
            ctx.HandleLogEntry character entry

        member this.whisper(target: string, message: string) =
            let entry = (LogType.Whisper, character.Character.Name, DateTime.Now, message)
            ctx.HandleLogEntry character entry

        member this.getLastWords() =
            // TODO get this somewhere?!
            // TODO with parameter? How many ticks in the past?!?
            [| "Test" |]

    type MemoryApi(character: CharacterState, ctx: WorldContext) =

        member this.getKnowledge() = character.Memory.Knowledge

        member this.getMemory() = character.Memory.PlayerMemory

        member this.getMemoryKeys() = character.Memory.PlayerMemory.Keys
        member this.getMemoryValue key = character.Memory.PlayerMemory[key]

        member this.setMemory(key, value: Object) =
            character.Memory.PlayerMemory[key] <- LuaUtil.ensureType value

    type WorldApi(character: CharacterState, ctx: WorldContext) =
        member this.getWorld() = ()

    type CharacterApi(character: CharacterState, ctx: WorldContext) =
        member this.getCharacter() = ()

    type ActionApi(character: CharacterState, ctx: WorldContext) =
        member this.getCurrentAction() = ()
