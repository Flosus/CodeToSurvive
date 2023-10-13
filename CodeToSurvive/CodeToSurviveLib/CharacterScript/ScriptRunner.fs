namespace CodeToSurviveLib.PlayerScript

open System
open System.Threading
open System.Threading.Tasks
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Action
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
        use cancellationTokenSource: CancellationTokenSource = new CancellationTokenSource()

        try
            async {
                cancellationTokenSource.CancelAfter(cancellationTime)
                return! playScript state
            }
        with :? TaskCanceledException ->
            async { return handleTimeout state }

    let RunScripts
        (ctx: WorldContext)
        (getScriptByPlayer: GetScriptByPlayer)
        (getJobByName: GetJob)
        (scriptRunTime: int)
        : WorldContext =
        let cancellationTime = TimeSpan.FromSeconds(scriptRunTime)

        let playerScripts =
            ctx.State.CharacterStates
            |> Array.map (fun pl ->
                (let script = getScriptByPlayer pl
                 (pl, script)))

        let asyncResults =
            playerScripts
            |> Array.map (fun (pState, pScript) -> runScript (pState, ctx) pScript cancellationTime)

        let results = asyncResults |> Async.Parallel |> Async.RunSynchronously

        let newStateData =
            results
            |> Array.map (fun (playerState, scriptResult) ->
                (let job = getJobByName scriptResult

                 (playerState,
                  { Character = playerState.Character
                    Action = job })))

        let playerStates = newStateData |> Array.map fst
        let PlayerJobStates = newStateData |> Array.map snd

        { ctx with
            State =
                { ctx.State with
                    CharacterStates = playerStates
                    ActiveActions = PlayerJobStates } }