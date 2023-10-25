namespace CodeToSurviveLib.Script

open System.IO
open System.Text
open System.Threading
open CodeToSurviveLib.CharacterScript
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

    let parseScriptResult (log: ILogger) (scriptResult: obj[]) : ScriptResult =
        match scriptResult.Length with
        | 0 ->
            log.LogDebug "Script has empty result"
            ScriptResult.Error "Script has returned no valid action."
        | _ ->
            let firstEntry = scriptResult[0] :?> LuaTable
            let actionName = firstEntry["Name"] :?> string

            match actionName with
            | null -> ScriptResult.Error "No Name found in the Result"
            | _ ->
                let actionParameter = firstEntry["Parameter"] :?> LuaTable

                match actionParameter with
                | null -> ScriptResult.Action(actionName, None)
                | _ ->
                    let mutable actionParams: obj[] = [||]

                    for param in actionParameter do
                        let paramDict = LuaUtil.ensureType param
                        actionParams <- actionParams |> Array.append [| paramDict |]

                    ScriptResult.Action(actionName, Some(actionParams))

    let generateCharacterScript (luaScript: string) : RunPlayerScript =

        let runScript (characterState: CharacterState) (ctx: WorldContext) (cancellationToken: CancellationToken) =
            let log = ctx.CreateLogger "LuaCharacterScript"
            use lua = new Lua()
            lua.LoadCLRPackage()
            lua.State.Encoding <- Encoding.UTF8
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
                        lua.State.Error("abort") |> ignore)

                lua.SetDebugHook(LuaHookMask.Count, 10000) |> ignore
                let scriptResultObj = lua.DoString(luaScript, "playerScript")
                let scriptResult = scriptResultObj |> parseScriptResult log
                let result = (characterState, scriptResult)
                result
            with ex when not cancellationToken.IsCancellationRequested ->
                log.LogError(
                    ex,
                    $"Error while running script for user {characterState.Character.Name}@{characterState.Character.Id}"
                )

                log.LogError $"StackTrace: {ex.StackTrace}"
                (characterState, ScriptResult.Error ex.Message)

        runScript
