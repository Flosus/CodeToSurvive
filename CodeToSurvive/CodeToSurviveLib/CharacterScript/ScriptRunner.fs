namespace CodeToSurviveLib.PlayerScript

open System
open System.Threading
open System.Threading.Tasks
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Script.ScriptInfo
open Microsoft.Extensions.Logging

module ScriptRunner =

    let handleTimeout (states: CharacterState * WorldContext) : CharacterState * ScriptResult =
        let charState, worldState = states
        let log = worldState.CreateLogger "ScriptTimeoutHandler"

        let message =
            $"CharScript timeout. CharacterId={charState.Character.Id}; Name={charState.Character.Name}"

        log.LogWarning message
        let entry = (LogType.System, "System", DateTime.Now, message)
        worldState.HandleLogEntry charState entry
        (charState, Timeout)

    let runScript (state: CharacterState * WorldContext) (playScript: RunPlayerScript) (timeout: int) =
        task {
            let playerScriptAsync =
                async {
                    use cancellationTokenSource = new CancellationTokenSource()
                    let cancellationTime = TimeSpan.FromMilliseconds(timeout)
                    cancellationTokenSource.CancelAfter(cancellationTime)
                    let token = cancellationTokenSource.Token
                    let charState, worldState = state
                    return playScript charState worldState token
                }

            use cancellationTokenSource = new CancellationTokenSource()
            let cancellationTime = TimeSpan.FromMilliseconds(timeout)
            cancellationTokenSource.CancelAfter(cancellationTime)
            let token = cancellationTokenSource.Token

            let result = Async.StartAsTask(playerScriptAsync, TaskCreationOptions.None, token)

            let! _ = Task.WhenAny(result, Task.Delay(Timeout.Infinite, token))

            try
                match result.IsCompleted with
                | true -> return! result
                | false -> return handleTimeout state
            with ex ->
                printfn $"Ex handled {ex}"
                return handleTimeout state
        }

    let runScripts
        (ctx: WorldContext)
        (getScriptByPlayer: GetScriptByPlayer)
        (getActionByName: GetAction)
        (scriptTimeout: int)
        : WorldContext =
        let log = ctx.CreateLogger "ScriptRunner"

        let playerScripts =
            ctx.State.CharacterStates
            |> Array.map (fun pl ->
                (let script = getScriptByPlayer pl
                 (pl, script)))

        let asyncResults =
            playerScripts
            |> Array.map (fun (pState, pScript) -> runScript (pState, ctx) pScript scriptTimeout)

        Task.WaitAll(asyncResults |> Seq.cast<Task> |> Array.ofSeq)

        let scriptResultToAction (charState, scriptResult) =
            let existing =
                ctx.State.ActiveActions
                |> Array.tryFind (fun actSub -> actSub.CharacterId = charState.Character.Id)

            match existing with
            | None ->
                let action = getActionByName charState scriptResult
                (charState, action)
            | Some existingAction when existingAction.IsCancelable ->
                log.LogInformation "Canceling existing action."
                // TODO notify cancel action
                let action = getActionByName charState scriptResult
                (charState, action)
            | Some existingAction ->
                // We can't cancel the action, please continue
                (charState, existingAction)


        let newStateData: (CharacterState * CharacterAction)[] =
            asyncResults
            |> Array.map (fun task -> task.Result)
            |> Array.map scriptResultToAction

        let playerStates = newStateData |> Array.map fst

        let mewPlayerActionStates = newStateData |> Array.map snd

        let playerActionStates =
            ctx.State.ActiveActions
            |> Array.filter (fun oldActions ->
                mewPlayerActionStates
                |> Array.exists (fun actSub -> actSub.CharacterId = oldActions.CharacterId)
                |> not)
            |> Array.append mewPlayerActionStates

        { ctx with
            State =
                { ctx.State with
                    CharacterStates = playerStates
                    ActiveActions = playerActionStates } }
