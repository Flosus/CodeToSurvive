namespace CodeToSurviveLib.Util

open System
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.World
open Microsoft.Extensions.Logging

module WorldContextUtil =

    let createDefaultWorldState () : WorldState =
        let state =
            { Timestamp = DateTime.Now
              Players = [||]
              Tasks = [||]
              Map = { Chunks = ResizeArray<Chunk>() } }

        state

    let loggerFactory (factory: ILoggerFactory) =
        fun (name: string) -> factory.CreateLogger $"CodeToSurvive.{name}"

    let createDefaultWorldContext (stateProvider: unit -> WorldState) (factory: ILoggerFactory) : WorldContext =
        let ctx =
            { CreateLogger = loggerFactory factory
              ProgressJob = fun (_, state) -> state
              RunCharacterScripts = fun state -> state
              PreTickUpdate = fun state -> state
              PostTickUpdate = fun state -> state
              State = stateProvider () }

        ctx

    let createDefaultCtx: ILoggerFactory -> WorldContext =
        createDefaultWorldContext createDefaultWorldState
