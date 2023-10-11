namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Character
open Microsoft.Extensions.Logging

module Tick =

    let rec doWithStateUpdate
        (char: CharacterState[])
        (state: WorldState)
        (act: CharacterState * WorldState -> WorldState)
        : WorldState =
        match char.Length with
        | 0 -> state
        | _ ->
            let currentPlayer = char[0]
            let newState = act (currentPlayer, state)
            let remainingChars = char[1..]
            doWithStateUpdate remainingChars newState act

    let doJobProgress (character: CharacterState[]) (state: WorldState) (act: DoJobProgress) : WorldState =
        let dJP (cha: CharacterState, state: WorldState) : WorldState =
            let findPlayerTask = fun (cur: PlayerTask) -> cha.Character.Id = cur.Character.Id
            let currentTask = state.Tasks |> Array.find findPlayerTask
            act (currentTask, state)

        doWithStateUpdate character state dJP

    let getPlayersWithoutJob (state: WorldState) =
        let filterPlayerHasNoJob (entry: CharacterState) =
            state.Tasks
            |> Array.filter (fun pt -> pt.Character = entry.Character && not pt.Job.IsCancelable)
            |> Array.isEmpty

        state.Players |> Array.filter filterPlayerHasNoJob

    let tick (context: WorldContext) : WorldContext =
        let log = context.CreateLogger("tick")
        log.LogDebug "Tick begin"

        let progressJobs (curState: WorldState) : WorldState =
            doJobProgress curState.Players curState context.ProgressJob

        let removeFinishedJobs (curState: WorldState) : WorldState =
            { curState with
                Tasks = curState.Tasks |> Array.filter isPlayerTaskOpen }

        let logStep msg localState =
            log.LogTrace msg
            localState
        
        let updateContext (state: WorldState): WorldContext =
            {context with State = state}

        context.State
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
        |> (fun state -> { state with Timestamp = DateTime.Now })
        |> logStep "Tick finished"
        |> updateContext
