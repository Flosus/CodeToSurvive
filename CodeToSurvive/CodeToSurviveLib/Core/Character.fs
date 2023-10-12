namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Item

module Character =

    type CharacterStats =
        { Hunger: int
          Thirst: int
          Fatigue: int }

    type Character =
        { Id: Guid
          Name: string
          PlayerStats: CharacterStats
          Inventory: ItemEntity[] }

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let newCharacter name =
        { Id = Guid.NewGuid()
          Name = name
          PlayerStats = NewPlayerStats
          Inventory = [||] }
