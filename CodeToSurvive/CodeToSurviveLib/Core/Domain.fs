namespace CodeToSurviveLib.Core

open System
open System.Collections.Generic
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging

module Domain =

    // General

    type Player = string
    type ChunkId = string
    type CharacterId = Guid
    type ActionName = string
    type CheckHandler = string

    type Stats =
        | Intelligence of int
        | Strength of int
        | Agility of int
        | BonusHealth of int
        | BonusEnergy of int
        | BonusMana of int

    type ActionDefinition =
        {
            ActionId: Guid
            Description: string
            DescriptionHandler: string
            ActionName: ActionName
            /// Optional specific action name
            /// E.g. The action is "Drink" and the ActionHandler is "DrinkBeer"
            ActionHandler: ActionName option
            CheckHandler: CheckHandler option
            HandlerParameter: Dictionary<string, obj>
        }

    // ITEM

    type Item =
        { ItemId: string
          Name: string
          Description: string
          DescriptionHandler: string option
          Weight: double
          Value: double
          Stateful: bool }

    type ItemTrigger =
        { OnPickup: string
          OnDrop: string
          OnEquip: string
          OnUnequip: string }

    type ItemEntity =
        { ItemId: Guid
          Item: Item
          Amount: double
          Trigger: ItemTrigger
          Actions: ActionDefinition[]
          State: Dictionary<string, string> }

    // World

    type Transition =
        { TargetMapId: string
          Description: string
          DescriptionHandler: string
          CheckHandler: CheckHandler }

    type POI =
        { Name: string
          Description: string
          DescriptionHandler: string
          Actions: ActionDefinition[] }

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
          Transitions: Transition[]
          POIs: POI[]
          State: ChunkState }


    /// A wrapper type used to contain the world, e.g. all chunks
    type WorldMap = { Chunks: ResizeArray<Chunk> }

    // Character

    type CharacterStats =
        { Hunger: int
          Thirst: int
          Fatigue: int }

    type CharacterMemory =
        { mutable Knowledge: (string * string)[]
          mutable PlayerMemory: Dictionary<string, obj> }

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

    type Character =
        { Id: CharacterId
          Name: string
          Player: Player
          mutable Location: ChunkId
          mutable CharacterStats: CharacterStats
          mutable Inventory: ItemEntity[]
          mutable Equipment: EntityEquipment }

    type CharacterState =
        { Character: Character
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

    type CharacterAction =
        { ActionId: Guid
          Name: ActionName
          ActionHandler: string
          CharacterId: CharacterId
          Duration: int
          mutable CurrentProgress: int
          mutable IsFinished: bool
          IsCancelable: bool
          // Ignore
          Parameter: obj[] option }

    let defaultCharacterAction =
        { ActionId = Guid.NewGuid()
          Name = "TODO"
          ActionHandler = "Idle"
          CharacterId = Guid.Empty
          Duration = 1
          CurrentProgress = 0
          IsFinished = false
          IsCancelable = false
          Parameter = None }

    (*
    ___________
    World state
    ___________
    This should get moved into it's own file after creating all missing states
    *)
    type WorldState =
        { mutable Timestamp: DateTime
          mutable CharacterStates: CharacterState[]
          mutable ActiveActions: CharacterAction[]
          Map: WorldMap }

    type WorldContext =
        { State: WorldState
          CreateLogger: string -> ILogger
          ProgressAction: CharacterAction -> WorldContext -> WorldContext
          OnStartup: WorldContext -> WorldContext
          PreTickUpdate: WorldContext -> WorldContext
          RunCharacterScripts: WorldContext -> WorldContext
          PostTickUpdate: WorldContext -> WorldContext
          StorageProvider: unit -> IStoragePreference
          HandleLogEntry: CharacterState -> LogEntry -> unit
          ScriptProvider: CharacterState -> string }
