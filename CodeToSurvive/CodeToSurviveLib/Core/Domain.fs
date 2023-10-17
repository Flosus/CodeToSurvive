namespace CodeToSurviveLib.Core

open System
open System.Collections.Generic
open System.Runtime.Serialization
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging

module Domain =

    // General

    type Player = string
    
    type ChunkId = string
    
    /// The name of the action. E.g. "Drink"
    type ActionName = string
    type CharacterId = Guid
    type CheckHandler = string option

    type Stats =
        | Intelligence of int
        | Strength of int
        | Agility of int
        | BonusHealth of int
        | BonusEnergy of int
        | BonusMana of int

    // ITEM

    type ItemTrigger = { OnDrop: string }

    type Item =
        { Name: string
          Type: string
          DefaultWeight: float
          DefaultStackSize: int }

    type ItemEntity =
        { Item: Item
          Amount: double
          Weight: double
          StackSize: int
          Trigger: ItemTrigger
          Metadata: Dictionary<string, string> }

    // World

    type Transition =
        { TargetMapId: string
          Description: string
          DescriptionHandler: string
          CheckHandler: CheckHandler }

    type WorldAction =
        { ActionId: string
          Description: string
          ActionName: ActionName
          ActionHandler: string option
          CheckHandler: CheckHandler
          HandlerParameter: Dictionary<string, obj> }

    type POI =
        { Name: string
          Description: string
          Actions: WorldAction [] }

    type ChunkState = Dictionary<string, obj>

    /// The abstract class of a chunk.
    /// Game Resource Plugins have to implement the specific Chunks
    type Chunk =
        { Id: ChunkId
          PluginId: string
          Name: string
          Description: string
          Instanced: bool
          Persistent: bool
          PersistencePool: string option
          Transitions: Transition []
          POIs: POI []
          State: ChunkState }


    /// A wrapper type used to contain the world, e.g. all chunks
    type WorldMap = { Chunks: ResizeArray<Chunk> }

    // Character

    [<DataContract>]
    type CharacterStats =
        { [<DataMember>]
          Hunger: int
          [<DataMember>]
          Thirst: int
          [<DataMember>]
          Fatigue: int }

    [<DataContract>]
    type CharacterMemory =
        { [<DataMember>]
          mutable Knowledge: (string * string) []
          [<DataMember>]
          mutable PlayerMemory: Dictionary<string, Object> }

    type EntityEquipment =
        { Chest: ItemEntity option
          Head: ItemEntity option
          Shoes: ItemEntity option
          // LeftArm + RightArm OR Arms
          LeftArm: ItemEntity option
          RightArm: ItemEntity option
          Arms: ItemEntity option
          // LeftLeg + RightLeg OR Legs
          LeftLeg: ItemEntity option
          RightLeg: ItemEntity option
          Legs: ItemEntity option
          Back: ItemEntity option
          LeftHand: ItemEntity option
          RightHand: ItemEntity option
          // Rings? Amulet? Glasses?
          Other: ItemEntity option }

    [<DataContract>]
    type Character =
        { [<DataMember>]
          Id: CharacterId
          [<DataMember>]
          Name: string
          [<DataMember>]
          Player: Player
          [<DataMember>]
          mutable Location: ChunkId
          [<DataMember>]
          mutable CharacterStats: CharacterStats
          [<DataMember>]
          mutable Inventory: ItemEntity []
          [<DataMember>]
          mutable Equipment: EntityEquipment }

    [<DataContract>]
    type CharacterState =
        { [<DataMember>]
          Character: Character
          Memory: CharacterMemory }

    // Log Messages

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

    // Actions

    type ActionParameter =
        | POI
        | Item
        // TODO change this to entity when NPCs are added?
        | Character
        | Transition
        | Text
        | ItemSlot

    [<DataContract>]
    type CharacterAction =
        { [<DataMember>]
          ActionId: String
          [<DataMember>]
          Name: ActionName
          [<DataMember>]
          ActionHandler: ActionName
          [<DataMember>]
          CharacterId: CharacterId
          [<DataMember>]
          Duration: int
          [<DataMember>]
          mutable CurrentProgress: int
          [<DataMember>]
          mutable IsFinished: bool
          [<DataMember>]
          IsCancelable: bool
          // Ignore
          Parameter: ActionParameter [] }

    (*
    ___________
    World state
    ___________
    This should get moved into it's own file after creating all missing states
    *)
    [<DataContract>]
    type WorldState =
        { [<DataMember>]
          mutable Timestamp: DateTime
          [<DataMember>]
          mutable CharacterStates: CharacterState []
          [<DataMember>]
          mutable ActiveActions: CharacterAction []
          [<DataMember>]
          Map: WorldMap }

    type WorldContext =
        { State: WorldState
          CreateLogger: string -> ILogger
          ProgressAction: CharacterAction * WorldContext -> WorldContext
          OnStartup: WorldContext -> WorldContext
          PreTickUpdate: WorldContext -> WorldContext
          RunCharacterScripts: WorldContext -> WorldContext
          PostTickUpdate: WorldContext -> WorldContext
          StorageProvider: unit -> IStoragePreference
          HandleLogEntry: CharacterState -> LogEntry -> unit
          ScriptProvider: CharacterState -> string }
