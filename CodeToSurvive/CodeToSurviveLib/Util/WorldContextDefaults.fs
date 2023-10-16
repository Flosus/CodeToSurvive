namespace CodeToSurviveLib.Util

open System
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.PlayerScript
open CodeToSurviveLib.Script
open CodeToSurviveLib.Script.ScriptInfo
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging

module WorldContextDefaults =
    open CodeToSurviveLib.Core.Plugin

    let scriptRunTime = 2000

    let rec private stateUpdate ctx (func: (WorldContext -> WorldContext)[]) : WorldContext =
        match func.Length with
        | 0 -> ctx
        | _ ->
            let newCtx = func[0] ctx
            stateUpdate newCtx func[1..]

    let private finPluginsWithAction
        (ctx: WorldContext)
        (funcMapper: PluginApi.Plugin -> Option<WorldContext -> WorldContext>)
        : WorldContext =
        let plugins = PluginRegistry.getPlugins ()

        let newCtx =
            plugins
            |> Array.map funcMapper
            |> Array.filter (fun funcOpt -> funcOpt.IsSome)
            |> Array.map (fun funcOpt -> funcOpt.Value)
            |> stateUpdate ctx

        newCtx

    let defaultPreTickUpdate (ctx: WorldContext) =
        finPluginsWithAction ctx (fun plugin -> plugin.PreTickUpdate)

    let defaultPostTickUpdate (ctx: WorldContext) =
        finPluginsWithAction ctx (fun plugin -> plugin.PostTickUpdate)

    let defaultRunCharacterScripts (ctx: WorldContext) =
        let scriptByPlayer (charState: CharacterState) : RunPlayerScript =
            charState.ScriptProvider() |> LuaCharacterScript.generateCharacterScript

        let getAction (charState: CharacterState) (scriptResult: ScriptResult) : CharacterAction =
            // TODO get action from script result
            CharacterAction.getIdleAction charState.Character.Id

        let newCtx = ScriptRunner.runScripts ctx scriptByPlayer getAction scriptRunTime
        finPluginsWithAction newCtx (fun plugin -> plugin.RunCharacterScripts)

    let defaultOnStartup (factory: ILoggerFactory) (ctx: WorldContext) =
        PluginRegistry.registerPlugins factory
        finPluginsWithAction ctx (fun plugin -> plugin.OnStartup)

    let createDefaultWorldState () : WorldState =
        let state =
            { Timestamp = DateTime.Now
              CharacterStates = [||]
              ActiveActions = [||]
              Map = { Chunks = ResizeArray<Chunk>() } }

        state

    let loggerFactory (factory: ILoggerFactory) =
        fun (name: string) -> factory.CreateLogger $"CodeToSurvive.{name}"

    let createDefaultWorldContext
        (stateProvider: unit -> WorldState)
        (factory: ILoggerFactory)
        storageProvider
        : WorldContext =
        let ctx =
            { CreateLogger = loggerFactory factory
              ProgressAction = snd
              OnStartup = defaultOnStartup factory
              RunCharacterScripts = defaultRunCharacterScripts
              PreTickUpdate = defaultPreTickUpdate
              PostTickUpdate = defaultPostTickUpdate
              State = stateProvider ()
              StorageProvider = storageProvider }

        ctx

    let createDefaultCtx: ILoggerFactory -> (unit -> IStoragePreference) -> WorldContext =
        createDefaultWorldContext createDefaultWorldState
