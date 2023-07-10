namespace CodeToSurvive.Lib.Script

open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Tick

module ScriptInfo =

    type ScriptResult =
        | JobName
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = PlayerState * State -> Async<PlayerState * ScriptResult>
    type GetScriptByPlayer = PlayerState -> RunPlayerScript
    type GetJob = ScriptResult -> Job
