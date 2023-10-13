namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Action
open CodeToSurviveLib.Core.Character
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Collections

module Tick =

    let rec private doWithContextUpdate
        (char: CharacterState[])
        (ctx: WorldContext)
        (act: CharacterState * WorldContext -> WorldContext)
        : WorldContext =
        match char.Length with
        | 0 -> ctx
        | _ ->
            let currentPlayer = char[0]
            let newState = act (currentPlayer, ctx)
            let remainingChars = char[1..]
            doWithContextUpdate remainingChars newState act

    let private doJobProgress (characters: CharacterState[]) (ctx: WorldContext) act : WorldContext =
        let dJP (cha: CharacterState, ctx: WorldContext) : WorldContext =
            let findPlayerTask =
                fun (cur: CharacterAction) -> cha.Character.Id = cur.Character.Id

            let currentTaskOpt = ctx.State.ActiveActions |> Array.tryFind findPlayerTask

            match currentTaskOpt with
            | Some currentTask -> act (currentTask, ctx)
            | None -> ctx

        doWithContextUpdate characters ctx dJP

    let tick (context: WorldContext) : WorldContext =
        let log = context.CreateLogger "Tick"
        log.LogDebug "Tick begin"

        let progressJobs (ctx: WorldContext) : WorldContext =
            let curState = ctx.State
            doJobProgress curState.CharacterStates ctx ctx.ProgressAction

        let removeFinishedJobs (ctx: WorldContext) : WorldContext =
            ctx.State.ActiveActions <- ctx.State.ActiveActions |> Array.filter isPlayerActionOpen
            ctx

        let logStep msg localState =
            log.LogTrace msg
            localState

        context
        // Pre tick work
        |> context.PreTickUpdate
        |> logStep "PreTickUpdate finished"
        // Run player scripts on what to do
        |> context.RunCharacterScripts
        |> logStep "RunCharacterScripts finished"
        // Execute the Jobs the player want to do
        |> progressJobs
        |> logStep "progressJobs finished"
        // Find not finished tasks
        |> removeFinishedJobs
        |> logStep "removeFinishedJobs finished"
        // Post tick work
        |> context.PostTickUpdate
        |> logStep "PostTickUpdate finished"
        |> (fun ctx ->
            ctx.State.Timestamp <- DateTime.Now
            ctx)
        |> logStep "Tick finished"
