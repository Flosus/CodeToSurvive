namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Item

module Character =

    type CharacterStats =
        { Hunger: int
          Thirst: int
          Fatigue: int }

    type Character =
        { Id: Guid
          Name: String
          PlayerStats: CharacterStats
          WorldMapPositionX: int
          WorldMapPositionY: int
          Inventory: ItemEntity[] }

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let newCharacter name =
        { Id = Guid.NewGuid()
          Name = name
          WorldMapPositionX = 0
          WorldMapPositionY = 0
          PlayerStats = NewPlayerStats
          Inventory = [||] }
