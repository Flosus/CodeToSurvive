namespace CodeToSurviveLib.Script

open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.GameState

module ScriptInfo =

    type ScriptResult =
        // TODO fix action parameter
        | Action of (string * string option)
        | Continue
        | Error
        | Timeout

    type RunPlayerScript = CharacterState * WorldContext -> Async<CharacterState * ScriptResult>
    type GetScriptByPlayer = CharacterState -> RunPlayerScript
    type GetAction = CharacterState -> ScriptResult -> CharacterAction.Action
