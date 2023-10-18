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

            let! res = Task.WhenAny(result, Task.Delay(Timeout.Infinite, token))

            try
                match result.IsCompleted with
                | true -> return! result
                | false ->
                    printfn "still not completed"
                    return handleTimeout state
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

        let playerScripts =
            ctx.State.CharacterStates
            |> Array.map (fun pl ->
                (let script = getScriptByPlayer pl
                 (pl, script)))

        let asyncResults =
            playerScripts
            |> Array.map (fun (pState, pScript) -> runScript (pState, ctx) pScript scriptTimeout)

        Task.WaitAll(asyncResults |> Seq.cast<Task> |> Array.ofSeq) |> ignore

        let scriptResultToAction
            (
                charState: CharacterState,
                scriptResult: ScriptResult
            ) : CharacterState * CharacterAction =
            let action: CharacterAction = getActionByName charState scriptResult

            (charState, action)

        let newStateData: (CharacterState * CharacterAction)[] =
            asyncResults
            |> Array.map (fun task -> task.Result)
            |> Array.map scriptResultToAction

        let playerStates = newStateData |> Array.map fst
        let PlayerActionStates = newStateData |> Array.map snd

        { ctx with
            State =
                { ctx.State with
                    CharacterStates = playerStates
                    ActiveActions = PlayerActionStates } }
