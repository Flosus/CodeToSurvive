namespace CodeToSurviveLib.Script

open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Action

module ScriptInfo =

    type ScriptResult =
        // TODO fix action parameter
        | Action of (string * string option)
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = CharacterState * WorldContext -> Async<CharacterState * ScriptResult>
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetAction = ScriptResult -> Action
