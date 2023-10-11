namespace CodeToSurvive.Lib.Script

open System.IO
open System.Text
open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Script.ScriptInfo
open NLua

module LuaPlayerScript =

    let getLuaPluginFiles () : string[] =
        let readFile path = File.ReadAllText(path, Encoding.UTF8)
        Directory.EnumerateFiles("./Data", "", SearchOption.AllDirectories)
        |> Seq.map readFile
        |> Seq.toArray

    let generatePlayerScript (luaScript: string) : RunPlayerScript =

        let runPlayerScript (characterState: CharacterState, ctx: WorldContext) =
            async {
                use lua = new Lua()
                // TODO setup context
                getLuaPluginFiles() |> Array.iter (fun libLua -> lua.DoString(libLua) |> ignore)
                // Disable import in scripts
                lua.DoString("import = function () end") |> ignore
                let scriptResult = lua.DoString(luaScript)
                // TODO update characterState
                // TODO parse script result for job
                let result = (characterState, ScriptResult.Continue)
                return result
            }

        runPlayerScript
