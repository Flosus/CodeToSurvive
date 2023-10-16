namespace CodeToSurviveLib.Core

open System
open System.Collections.Generic
open System.Runtime.Serialization
open CodeToSurviveLib.Core.Domain

module Character =

    let NewPlayerStats = { Hunger = 0; Thirst = 0; Fatigue = 0 }

    let internal newCharacter name player getSpawnChunk =
        { Id = Guid.NewGuid()
          Name = name
          Player = player
          CharacterStats = NewPlayerStats
          Location = getSpawnChunk ()
          Inventory = [||] }
