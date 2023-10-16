namespace CodeToSurviveLib.Script

open System
open System.IO
open System.Net.WebSockets
open System.Text
open CodeToSurviveLib.CharacterScript.Api
open CodeToSurviveLib.CharacterScript.Api.ScriptApi
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Script.ScriptInfo
open NLua
open NLua.Exceptions
open Microsoft.Extensions.Logging

module LuaCharacterScript =

    let getLuaPluginFiles subPath : (string*string)[] =
        let readFile path = (File.ReadAllText(path, Encoding.UTF8), Path.GetFileName path)

        Directory.EnumerateFiles(subPath, "*.lua", SearchOption.AllDirectories)
        |> Seq.map readFile
        |> Seq.sortBy snd
        |> Seq.toArray

    let generateCharacterScript (luaScript: string) : RunPlayerScript =

        let runScript (characterState: CharacterState,
                       ctx: WorldContext) =
            async {
                let log = ctx.CreateLogger "LuaCharacterScript"
                use lua = new Lua()
                lua.LoadCLRPackage()
                // Setup Api implementation
                lua.SetObjectToPath("_api_log", LoggingApi(characterState, ctx))
                lua.SetObjectToPath("_api_communication", CommunicationApi(characterState, ctx))
                lua.SetObjectToPath("_api_memory", MemoryApi(characterState, ctx))
                lua.SetObjectToPath("_api_world", WorldApi(characterState, ctx))
                lua.SetObjectToPath("_api_character", CharacterApi(characterState, ctx))
                lua.SetObjectToPath("_api_action", ActionApi(characterState, ctx))
                // TODO how to add custom functions from plugins, e.g. plugin api
                // Setup Lua Api
                getLuaPluginFiles "./Data"
                |> Array.iter (fun (luaScript, scriptName) ->
                    lua.DoString(luaScript, scriptName) |> ignore
                    log.LogTrace $"Registering lua script {scriptName}")
                // Disable import in scripts
                lua.DoString("import = function () end") |> ignore
                try
                    let scriptResult = lua.DoString(luaScript, "playerScript")
                    // TODO update characterState
                    // TODO parse script result for action
                    let result = (characterState, ScriptResult.Continue)
                    return result
                with ex ->
                    log.LogError (ex, $"Error while running script for user {characterState.Character.Name}@{characterState.Character.Id}")
                    log.LogError $"StackTrace: {ex.StackTrace}"
                    characterState.HandleLogEntry (LogType.System, "Script", DateTime.Now, ex.Message)
                    return (characterState, ScriptResult.Error)
            }

        runScript
