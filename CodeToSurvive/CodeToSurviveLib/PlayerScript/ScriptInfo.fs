namespace CodeToSurvive.Lib.Script

open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Job

module ScriptInfo =

    type ScriptResult =
        // TODO fix job parameter
        | Job of (string * string option)
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = CharacterState * WorldContext -> Async<CharacterState * ScriptResult>
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetJob = ScriptResult -> Job
