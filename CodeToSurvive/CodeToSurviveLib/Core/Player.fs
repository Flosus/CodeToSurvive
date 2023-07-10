namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Item

module Player =

    type PlayerStats =
        { Hunger: int
          Thirst: int
          Fatigue: int }

    type Player =
        { Id: Guid
          Name: String
          PlayerStats: PlayerStats
          WorldMapPositionX: int
          WorldMapPositionY: int
          Inventory: ItemEntity[] }

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let NewPlayer name =
        { Id = Guid.NewGuid()
          Name = name
          WorldMapPositionX = 0
          WorldMapPositionY = 0
          PlayerStats = NewPlayerStats
          Inventory = [||] }
