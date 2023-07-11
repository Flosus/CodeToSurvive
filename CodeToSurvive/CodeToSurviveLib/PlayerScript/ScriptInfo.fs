namespace CodeToSurvive.Lib.Script

open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Tick

module ScriptInfo =

    type ScriptResult =
        | JobName
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = CharacterState * State -> Async<CharacterState * ScriptResult>
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetJob = ScriptResult -> Job
