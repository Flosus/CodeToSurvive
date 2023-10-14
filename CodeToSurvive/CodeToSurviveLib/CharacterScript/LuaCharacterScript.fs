namespace CodeToSurviveLib.Script

open System.IO
open System.Text
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Script.ScriptInfo
open NLua

module LuaCharacterScript =

    let getLuaPluginFiles () : string[] =
        let readFile path = File.ReadAllText(path, Encoding.UTF8)

        Directory.EnumerateFiles("./Data", "*.lua", SearchOption.AllDirectories)
        |> Seq.map readFile
        |> Seq.toArray

    let generateCharacterScript (luaScript: string) : RunPlayerScript =

        let runScript (characterState: CharacterState, ctx: WorldContext) =
            async {
                use lua = new Lua()
                // TODO setup context
                getLuaPluginFiles ()
                |> Array.iter (fun libLua -> lua.DoString(libLua) |> ignore)
                // Disable import in scripts
                lua.DoString("import = function () end") |> ignore
                let scriptResult = lua.DoString(luaScript)
                // TODO update characterState
                // TODO parse script result for action
                let result = (characterState, ScriptResult.Continue)
                return result
            }

        runScript
