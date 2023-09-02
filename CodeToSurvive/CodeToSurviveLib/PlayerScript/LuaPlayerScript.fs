namespace CodeToSurvive.Lib.Script

open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Script.ScriptInfo
open NLua

module LuaPlayerScript =

    let generatePlayerScript (luaScript: string) : RunPlayerScript =

        let runPlayerScript (characterState: CharacterState, worldState: WorldState) =
            async {
                use lua = Lua()
                let scriptResult = ScriptResult.Continue
                let result = (characterState, scriptResult)
                return result
            }

        runPlayerScript
