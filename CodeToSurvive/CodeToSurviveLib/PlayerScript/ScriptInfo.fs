namespace CodeToSurvive.Lib.Script

open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Job

module ScriptInfo =

    type ScriptResult =
        | JobName
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = CharacterState * WorldState -> Async<CharacterState * ScriptResult>
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetJob = ScriptResult -> Job
