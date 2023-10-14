namespace CodeToSurviveLib.Core

open System
open System.Runtime.Serialization
open CodeToSurviveLib.Core.Item
open CodeToSurviveLib.Core.Player.PlayerManager
open CodeToSurviveLib.Core.World

module Character =

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
          Id: Guid
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

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let internal newCharacter name player getSpawnChunk =
        { Id = Guid.NewGuid()
          Name = name
          Player = player
          CharacterStats = NewPlayerStats
          Location = getSpawnChunk ()
          Inventory = [||] }
