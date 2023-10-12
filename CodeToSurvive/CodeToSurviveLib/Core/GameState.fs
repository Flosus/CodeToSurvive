namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Job
open CodeToSurviveLib.Core.Character
open CodeToSurviveLib.Core.World
open Microsoft.Extensions.Logging

module GameState =

    type CharacterState = { Character: Character }

    type WorldState =
        { Timestamp: DateTime
          Players: CharacterState[]
          Tasks: PlayerTask[]
          Map: WorldMap }

    type DoJobProgress = PlayerTask * WorldState -> WorldState
    type RunCharacterScripts = WorldState -> WorldState
    type StateUpdate = WorldState -> WorldState

    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressJob: DoJobProgress
          RunCharacterScripts: RunCharacterScripts
          PreTickUpdate: StateUpdate
          PostTickUpdate: StateUpdate
          State: WorldState }
