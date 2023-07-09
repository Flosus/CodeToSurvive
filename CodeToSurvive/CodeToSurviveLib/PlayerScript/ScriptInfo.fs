namespace CodeToSurvive.Lib.Script

open System
open System.Threading
open System.Threading.Tasks
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
