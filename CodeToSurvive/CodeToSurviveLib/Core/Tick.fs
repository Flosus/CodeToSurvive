namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Job
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

    let private doJobProgress (character: CharacterState[]) (ctx: WorldContext) (act) : WorldContext =
        let dJP (cha: CharacterState, ctx: WorldContext) : WorldContext =
            let findPlayerTask = fun (cur: PlayerTask) -> cha.Character.Id = cur.Character.Id
            let currentTask = ctx.State.Tasks |> Array.find findPlayerTask
            act (currentTask, ctx)

        doWithContextUpdate character ctx dJP

    let tick (context: WorldContext) : WorldContext =
        let log = context.CreateLogger "Tick"
        log.LogDebug "Tick begin"

        let progressJobs (ctx: WorldContext) : WorldContext =
            let curState = ctx.State
            doJobProgress curState.Players ctx ctx.ProgressJob

        let removeFinishedJobs (ctx: WorldContext) : WorldContext =
            ctx.State.Tasks <- ctx.State.Tasks |> Array.filter isPlayerTaskOpen
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
