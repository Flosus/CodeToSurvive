namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Item
open CodeToSurvive.Lib.Core.Position

module Character =

    type CharacterStats =
        { Hunger: int
          Thirst: int
          Fatigue: int }

    type Character =
        { Id: Guid
          Name: string
          PlayerStats: CharacterStats
          PlayerPosition: AbsoluteTilePosition
          Inventory: ItemEntity[] }

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let newCharacter name =
        { Id = Guid.NewGuid()
          Name = name
          PlayerPosition = { X = 0; Y = 0 }
          PlayerStats = NewPlayerStats
          Inventory = [||] }
