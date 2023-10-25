namespace CodeToSurviveLib.Script

open System.Threading
open CodeToSurviveLib.Core.Domain

module ScriptInfo =

    type ScriptResult =
        | Action of (string * obj[] option)
        | Continue
        | Error of string
        | Timeout

    type RunPlayerScript = CharacterState -> WorldContext -> CancellationToken -> CharacterState * ScriptResult
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetAction = CharacterState -> ScriptResult -> CharacterAction
