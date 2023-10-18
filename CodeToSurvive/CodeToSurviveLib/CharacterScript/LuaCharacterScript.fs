namespace CodeToSurviveLib.Script

open System
open System.IO
open System.Text
open System.Threading
open CodeToSurviveLib.CharacterScript.Api.ScriptApi
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Script.ScriptInfo
open KeraLua
open NLua
open Microsoft.Extensions.Logging

module LuaCharacterScript =

    let getLuaPluginFiles subPath : (string * string)[] =
        let readFile path =
            (File.ReadAllText(path, Encoding.UTF8), Path.GetFileName path)

        Directory.EnumerateFiles(subPath, "*.lua", SearchOption.AllDirectories)
        |> Seq.map readFile
        |> Seq.sortBy snd
        |> Seq.toArray

    let generateCharacterScript (luaScript: string) : RunPlayerScript =

        let runScript (characterState: CharacterState) (ctx: WorldContext) (cancellationToken: CancellationToken) =
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
                lua.DebugHook.Add(fun _ ->
                    if cancellationToken.IsCancellationRequested then
                        lua.State.Error() |> ignore)

                lua.SetDebugHook(LuaHookMask.Count, 100) |> ignore
                let scriptResult = lua.DoString(luaScript, "playerScript")
                // TODO update characterState
                // TODO parse script result for action
                let result = (characterState, ScriptResult.Continue)
                result
            with ex when not cancellationToken.IsCancellationRequested ->
                log.LogError(
                    ex,
                    $"Error while running script for user {characterState.Character.Name}@{characterState.Character.Id}"
                )

                log.LogError $"StackTrace: {ex.StackTrace}"
                let entry = (LogType.System, "Script", DateTime.Now, ex.Message)
                ctx.HandleLogEntry characterState entry
                (characterState, ScriptResult.Error)

        runScript
