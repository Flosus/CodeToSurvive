namespace CodeToSurviveLib.PlayerScript

open System
open System.Threading
open System.Threading.Tasks
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Script.ScriptInfo

module ScriptRunner =

    let handleTimeout (states: CharacterState * WorldContext) : CharacterState * ScriptResult =
        let playerState, _ = states
        // TODO write messages, etc
        (playerState, Timeout)

    let runScript
        (state: CharacterState * WorldContext)
        (playScript: RunPlayerScript)
        (cancellationTime: TimeSpan)
        : Async<CharacterState * ScriptResult> =

        try
            async {
                use cancellationTokenSource: CancellationTokenSource = new CancellationTokenSource()
                cancellationTokenSource.CancelAfter(cancellationTime)
                return! playScript state
            }
        with :? TaskCanceledException ->
            async { return handleTimeout state }

    let runScripts
        (ctx: WorldContext)
        (getScriptByPlayer: GetScriptByPlayer)
        (getActionByName: GetAction)
        (scriptTimeout: int)
        : WorldContext =
        let cancellationTime = TimeSpan.FromSeconds(scriptTimeout)

        let playerScripts =
            ctx.State.CharacterStates
            |> Array.map (fun pl ->
                (let script = getScriptByPlayer pl
                 (pl, script)))

        let asyncResults =
            playerScripts
            |> Array.map (fun (pState, pScript) -> runScript (pState, ctx) pScript cancellationTime)

        let results = asyncResults |> Async.Parallel |> Async.RunSynchronously

        let scriptResultToAction
            (
                charState: CharacterState,
                scriptResult: ScriptResult
            ) : CharacterState * CharacterAction.Action =
            let action: CharacterAction.Action = getActionByName charState scriptResult

            (charState, action)

        let newStateData: (CharacterState * CharacterAction.Action)[] =
            results |> Array.map scriptResultToAction

        let playerStates = newStateData |> Array.map fst
        let PlayerActionStates = newStateData |> Array.map snd

        { ctx with
            State =
                { ctx.State with
                    CharacterStates = playerStates
                    ActiveActions = PlayerActionStates } }
