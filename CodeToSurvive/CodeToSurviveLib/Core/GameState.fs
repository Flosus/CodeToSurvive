namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Character
open CodeToSurvive.Lib.Core.World
open Microsoft.Extensions.Logging

module GameState =

    type CharacterState = { Character: Character }

    type WorldState =
        { Timestamp: DateTime
          Players: CharacterState[]
          Tasks: PlayerTask[]
          Map: WorldMap }

    type RunCharacterScripts = WorldState -> WorldState
    type DoJobProgress = PlayerTask * WorldState -> WorldState
    type StateUpdate = WorldState -> WorldState

    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressJob: DoJobProgress
          RunCharacterScripts: RunCharacterScripts
          PreTickUpdate: StateUpdate
          PostTickUpdate: StateUpdate }
