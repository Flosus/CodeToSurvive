namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Job
open CodeToSurviveLib.Core.Character
open CodeToSurviveLib.Core.World
open Microsoft.Extensions.Logging

module GameState =

    type CharacterState = { Character: Character }

    type WorldState =
        { mutable Timestamp: DateTime
          mutable Players: CharacterState[]
          mutable Tasks: PlayerTask[]
          Map: WorldMap }
      
    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressJob: PlayerTask * WorldContext -> WorldContext
          OnStartup: WorldContext -> WorldContext
          RunCharacterScripts: WorldContext -> WorldContext
          PreTickUpdate: WorldContext -> WorldContext
          PostTickUpdate: WorldContext -> WorldContext
          State: WorldState }
