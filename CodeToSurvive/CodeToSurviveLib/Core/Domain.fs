namespace CodeToSurviveLib.Core

open System
open System.Collections.Generic
open System.Runtime.Serialization
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging

module Domain =

    // General

    type Player = string

    type Intelligence = int
    type Strength = int
    type Agility = int

    type Stats =
        | Intelligence
        | Strength
        | Agility

    type ChunkId = string
    type ActionName = string

    // ITEM

    type Item(name: string) =
        member this.Name = name
        member this.Type = ""
        member this.DefaultWeight = 0.0
        member this.DefaultStackSize = 1

    type ItemEntity =
        { Item: Item
          Amount: double
          Weight: double
          StackSize: int
          // TODO how to save metadata
          Metadata: Dictionary<string, string> }

    // Equipment

    type EntityStats =
        { Intelligence: Intelligence
          Strength: Strength
          Agility: Agility }

    type EntityEquipment =
        { Chest: ItemEntity
          Head: ItemEntity
          Shoes: ItemEntity
          // LeftArm + RightArm OR Arms
          LeftArm: ItemEntity
          RightArm: ItemEntity
          Arms: ItemEntity
          // LeftLeg + RightLeg OR Legs
          LeftLeg: ItemEntity
          RightLeg: ItemEntity
          Legs: ItemEntity
          Back: ItemEntity
          LeftHand: ItemEntity
          RightHand: ItemEntity
          // Rings? Amulet? Glasses?
          Other: ItemEntity }

    // World


    type ChunkState = Dictionary<string, obj>

    type WorldAction =
        { ActionId: string
          Description: string
          ActionName: ActionName
          ActionHandler: string
          CheckHandler: string
          HandlerParameter: Dictionary<string, obj> }

    type TransitionDefinition =
        { TargetMapId: string
          Description: string
          DescriptionHandler: string
          Check: string }

    type POI =
        { Name: string
          Description: string
          Actions: WorldAction[] }

    // TODO create real function references
    type Trigger =
        { mutable OnMapEnter: string
          mutable OnMapExit: string
          mutable OnMapStay: string
          mutable OnMapSave: string
          mutable OnMapLoad: string
          mutable OnMapUnload: string }

    /// The abstract class of a chunk.
    /// Game Resource Plugins have to implement the specific Chunks
    type Chunk(id: ChunkId, pluginId: string, name: string, description: string) =
        member this.Id = id
        member this.PluginId = pluginId
        member this.Name = name
        member this.Description = description
        // member val Instanced: bool = false with get, set
        // member val Persistent: bool = false with get, set
        // member val PersistencePool: string option = None with get, set
        // member val Transitions: TransitionDefinition[] = [||] with get, set
        // // member val   Trigger: Trigger with get, set
        // member val POIs: POI[] = [||] with get, set

        member this.State = ChunkState()

    /// A wrapper type used to contain the world, e.g. all chunks
    type WorldMap = { Chunks: ResizeArray<Chunk> }


    // Character

    type CharacterId = Guid

    [<DataContract>]
    type CharacterStats =
        { [<DataMember>]
          Hunger: int
          [<DataMember>]
          Thirst: int
          [<DataMember>]
          Fatigue: int }

    [<DataContract>]
    type Character =
        { [<DataMember>]
          Id: CharacterId
          [<DataMember>]
          Name: string
          [<DataMember>]
          Player: Player
          [<DataMember>]
          Location: ChunkId
          [<DataMember>]
          mutable CharacterStats: CharacterStats
          [<DataMember>]
          mutable Inventory: ItemEntity[] }

    [<DataContract>]
    type CharacterMemory =
        { [<DataMember>]
          mutable Knowledge: (string * string)[]
          [<DataMember>]
          mutable PlayerMemory: Dictionary<string, Object> }

    // Action
    type ActionParameter =
        | POI of POI
        | Item of Item
        // TODO change this to entity when NPCs are added?
        | Character of CharacterId
        | Transition of TransitionDefinition
        | Text of string
        // TODO ItemSlot Enum?
        | ItemSlot of string

    [<DataContract>]
    type CharacterAction =
        { [<DataMember>]
          ActionId: String
          [<DataMember>]
          CharacterId: CharacterId
          [<DataMember>]
          Name: ActionName
          [<DataMember>]
          Duration: int
          [<DataMember>]
          mutable CurrentProgress: int
          [<DataMember>]
          mutable IsFinished: bool
          [<DataMember>]
          IsCancelable: bool
          // Ignore
          Parameter: ActionParameter[] }

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
          mutable ActiveActions: CharacterAction[]
          [<DataMember>]
          Map: WorldMap }

    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressAction: CharacterAction * WorldContext -> WorldContext
          OnStartup: WorldContext -> WorldContext
          PreTickUpdate: WorldContext -> WorldContext
          RunCharacterScripts: WorldContext -> WorldContext
          PostTickUpdate: WorldContext -> WorldContext
          State: WorldState
          StorageProvider: unit -> IStoragePreference }
