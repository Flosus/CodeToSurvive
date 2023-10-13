namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Action
open CodeToSurviveLib.Core.Character
open CodeToSurviveLib.Core.World
open Microsoft.Extensions.Logging

module GameState =

    type CharacterState =
        { Character: Character
        // TODO characterLogHandler?
        // TODO characterScriptHandler?
        }

    type WorldState =
        { mutable Timestamp: DateTime
          mutable CharacterStates: CharacterState[]
          mutable ActiveActions: CharacterAction[]
          Map: WorldMap }

    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressAction: CharacterAction * WorldContext -> WorldContext
          OnStartup: WorldContext -> WorldContext
          RunCharacterScripts: WorldContext -> WorldContext
          PreTickUpdate: WorldContext -> WorldContext
          PostTickUpdate: WorldContext -> WorldContext
          State: WorldState }
