namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Item
open CodeToSurviveLib.Core.Player.PlayerManager

module Character =

    type CharacterStats =
        { Hunger: int
          Thirst: int
          Fatigue: int }

    type Character =
        { Id: Guid
          Name: string
          Player: Player
          mutable CharacterStats: CharacterStats
          mutable Inventory: ItemEntity[] }

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let internal newCharacter name player =
        { Id = Guid.NewGuid()
          Name = name
          Player = player
          CharacterStats = NewPlayerStats
          Inventory = [||] }
