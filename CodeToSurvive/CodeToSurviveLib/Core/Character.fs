namespace CodeToSurviveLib.Core

open System
open System.Collections.Generic
open System.Runtime.Serialization
open CodeToSurviveLib.Core.Item
open CodeToSurviveLib.Core.Player.PlayerManager
open CodeToSurviveLib.Core.World

module Character =

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

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let internal newCharacter name player getSpawnChunk =
        { Id = Guid.NewGuid()
          Name = name
          Player = player
          CharacterStats = NewPlayerStats
          Location = getSpawnChunk ()
          Inventory = [||] }
