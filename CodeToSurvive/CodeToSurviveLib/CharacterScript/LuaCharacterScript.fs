namespace CodeToSurviveLib.Script

open System.IO
open System.Text
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Script.ScriptInfo
open NLua
open NLua.Exceptions
open Microsoft.Extensions.Logging

module LuaCharacterScript =

    let getLuaPluginFiles subPath : string[] =
        let readFile path = File.ReadAllText(path, Encoding.UTF8)

        Directory.EnumerateFiles(subPath, "*.lua", SearchOption.AllDirectories)
        |> Seq.map readFile
        |> Seq.toArray

    let generateCharacterScript (luaScript: string) : RunPlayerScript =

        let runScript (characterState: CharacterState,
                       ctx: WorldContext) =
            async {
                let log = ctx.CreateLogger "LuaCharacterScript"
                use lua = new Lua()
                // TODO setup context
                getLuaPluginFiles "./Data"
                |> Array.iter (fun libLua -> lua.DoString(libLua) |> ignore)
                // Disable import in scripts
                lua.DoString("import = function () end") |> ignore
                try
                    let scriptResult = lua.DoString(luaScript)
                    // TODO update characterState
                    // TODO parse script result for action
                    let result = (characterState, ScriptResult.Continue)
                    return result
                with :? LuaScriptException as lse ->
                    log.LogError (lse, $"Error while running script for user {characterState.Character.Name}@{characterState.Character.Id}")
                    // TODO add message to user
                    return (characterState, ScriptResult.Error)
            }

        runScript
