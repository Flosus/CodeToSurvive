namespace CodeToSurviveLib.Script

open System.Threading
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.Domain

module ScriptInfo =

    type ScriptResult =
        // TODO fix action parameter
        | Action of (string * string option)
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = CharacterState -> WorldContext -> CancellationToken -> CharacterState * ScriptResult
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetAction = CharacterState -> ScriptResult -> CharacterAction
