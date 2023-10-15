namespace CodeToSurviveLib.Core

open System
open System.Runtime.Serialization
open CodeToSurviveLib.Core.CharacterAction
open CodeToSurviveLib.Core.Character
open CodeToSurviveLib.Core.World
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging

module GameState =

    type LogType =
        | Whisper
        | Say
        | Yell
        | System
        | Thought
        | Thonk

    type LogSource = string
    type LogMessage = string
    type LogEntry = LogType * LogSource * DateTime * LogMessage
    type HandleLogEntry = LogEntry -> unit

    [<DataContract>]
    type CharacterState =
        { [<DataMember>]
          Character: Character
          Memory: CharacterMemory
          // TODO recreate functions on restore
          [<IgnoreDataMember>]
          HandleLogEntry: HandleLogEntry
          [<IgnoreDataMember>]
          ScriptProvider: unit -> string }

    [<DataContract>]
    type WorldState =
        { [<DataMember>]
          mutable Timestamp: DateTime
          [<DataMember>]
          mutable CharacterStates: CharacterState[]
          [<DataMember>]
          mutable ActiveActions: Action[]
          [<DataMember>]
          Map: WorldMap }

    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressAction: Action * WorldContext -> WorldContext
          OnStartup: WorldContext -> WorldContext
          RunCharacterScripts: WorldContext -> WorldContext
          PreTickUpdate: WorldContext -> WorldContext
          PostTickUpdate: WorldContext -> WorldContext
          State: WorldState
          StorageProvider: unit -> IStoragePreference }
