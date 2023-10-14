namespace CodeToSurviveResource.BasePlugin

open System
open CodeToSurviveLib.Core.GameState
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module PostTickUpdate =

    let postTickUpdate (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.PostTickUpdate"
        log.LogTrace $"Did something"
        let logEntry = (LogType.System, "BasePlugin", DateTime.Now, "A message")
        ctx.State.CharacterStates |> Array.iter (fun char -> char.HandleLogEntry logEntry)
        ctx
