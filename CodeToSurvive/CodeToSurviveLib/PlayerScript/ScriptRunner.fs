namespace CodeToSurvive.Lib.PlayerScript

open System
open System.Threading
open System.Threading.Tasks
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Script.ScriptInfo

module ScriptRunner =

    let handleTimeout (states: CharacterState * State) : CharacterState * ScriptResult =
        let playerState, _ = states
        // TODO write messages, etc
        (playerState, Timeout)

    let runScript
        (state: CharacterState * State)
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
        (state: State)
        (getScriptByPlayer: GetScriptByPlayer)
        (getJobByName: GetJob)
        (scriptRunTime: int)
        : State =
        let cancellationTime = TimeSpan.FromSeconds(scriptRunTime)

        let playerScripts =
            state.Players
            |> Array.map (fun pl ->
                (let script = getScriptByPlayer pl
                 (pl, script)))

        let asyncResults =
            playerScripts
            |> Array.map (fun (pState, pScript) -> runScript (pState, state) pScript cancellationTime)

        let results = asyncResults |> Async.Parallel |> Async.RunSynchronously

        let newStateData =
            results
            |> Array.map (fun (playerState, scriptResult) ->
                (let job = getJobByName scriptResult

                 (playerState,
                  { Character = playerState.Character
                    Job = job })))

        let playerStates = newStateData |> Array.map fst
        let PlayerJobStates = newStateData |> Array.map snd

        { state with
            Players = playerStates
            Tasks = PlayerJobStates }
