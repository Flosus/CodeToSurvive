namespace CodeToSurviveLib.Script

open System
open System.Threading
open CodeToSurviveLib.Core.Domain

module ScriptInfo =

    type ScriptResult =
        | Action of (string * Object[] option)
        | Continue
        | Error of string
        | Timeout

    type RunPlayerScript = CharacterState -> WorldContext -> CancellationToken -> CharacterState * ScriptResult
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetAction = CharacterState -> ScriptResult -> CharacterAction
