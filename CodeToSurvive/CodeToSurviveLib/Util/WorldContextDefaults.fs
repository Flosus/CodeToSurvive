namespace CodeToSurviveLib.Util

open System
open System.IO
open System.Text
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin.PluginApi
open CodeToSurviveLib.PlayerScript
open CodeToSurviveLib.Script
open CodeToSurviveLib.Script.ScriptInfo
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging
open CodeToSurviveLib.Core.Plugin

module WorldContextDefaults =

    let logEntryToText entry = $"{entry}\n"

    let scriptRunTime = 2000


    let handleIdleAction (ctx: WorldContext) (charAction: CharacterAction) =
        let log = ctx.CreateLogger "Idle handler"
        charAction.CurrentProgress <- charAction.CurrentProgress + 1
        log.LogTrace $"handleIdleAction {charAction.CurrentProgress} @ {charAction.ActionId}"
        ctx

    let provideIdleAction (ctx: WorldContext) : ActionProvider =
        let log = ctx.CreateLogger "Idle provider"
        let produce (state, (name: string, parameter)) =
            match name.ToLower() with
            | "idle" ->
                log.LogTrace $"providing idle action for {state.Character.Name}"
                Some(
                    { defaultCharacterAction with
                        Name = "Idle"
                        ActionHandler = "Idle"
                        CharacterId = state.Character.Id
                        Duration = 1
                        Parameter = parameter }
                )
            | _ -> None

        produce

    let rec private stateUpdate ctx (func: (WorldContext -> WorldContext)[]) : WorldContext =
        match func.Length with
        | 0 -> ctx
        | _ ->
            let newCtx = func[0]ctx
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
        let log = ctx.CreateLogger "Context"

        let scriptByPlayer (charState: CharacterState) : RunPlayerScript =
            charState |> ctx.ScriptProvider |> LuaCharacterScript.generateCharacterScript

        let getAction (charState: CharacterState) (scriptResult: ScriptResult) : CharacterAction =
            let logMessage msg =
                (LogType.Thought, charState.Character.Name, DateTime.Now, msg)
                |> ctx.HandleLogEntry charState

            let getIdleState () =
                let idleActionProvider = provideIdleAction ctx
                let idle = idleActionProvider (charState, ("Idle", None))

                match idle with
                | None -> raise (Exception("Idle not found!"))
                | Some x -> x

            match scriptResult with
            | Continue ->
                match
                    ctx.State.ActiveActions
                    |> Array.tryFind (fun activeAct -> activeAct.CharacterId = charState.Character.Id)
                with
                | Some activeAct -> activeAct
                | None -> getIdleState ()
            | Error errorMessage ->
                logMessage $"I had a headache while trying to decide what to do: {errorMessage}"
                getIdleState ()
            | Timeout ->
                logMessage "I couldn't decide what to do."
                getIdleState ()
            | Action(name, actionParams) ->
                match PluginRegistry.getActionProvider (charState, (name, actionParams)) with
                | None ->
                    logMessage $"I can't do {name}. Parameter={actionParams}"
                    log.LogWarning $"Action not found: {name}; {actionParams}"
                    getIdleState ()
                | Some action -> action

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

        let logHandler (charState: CharacterState) (entry: LogEntry) =
            let (storage: IStoragePreference) = storageProvider ()

            let playerStorage =
                storage.PlayerStorageFolder.CreateSubdirectory $"{charState.Character.Name}"

            let logStorage = playerStorage.CreateSubdirectory "Log"
            let logFile = Path.Join(logStorage.FullName, "player.log")
            let entryTxt = logEntryToText entry
            lock charState (fun () -> File.AppendAllText(logFile, entryTxt, Encoding.UTF8))
            ()

        let scriptProvider (charState: CharacterState) =
            let storage = storageProvider ()

            let playerStorage =
                storage.PlayerStorageFolder.CreateSubdirectory $"{charState.Character.Name}"

            let scriptStorage = playerStorage.CreateSubdirectory "Script"

            LuaCharacterScript.getLuaPluginFiles scriptStorage.FullName
            |> Array.map fst
            |> String.concat "\n"

        let progressActions (charAction: CharacterAction) (ctx: WorldContext) : WorldContext =
            let handlerOpt = PluginRegistry.findAction charAction.ActionHandler

            match handlerOpt with
            | Some handler -> handler ctx charAction
            | None ->
                let log = ctx.CreateLogger "Context"
                log.LogWarning $"No handler found for action {charAction.ActionHandler}"
                ctx

        let ctx =
            { CreateLogger = loggerFactory factory
              ProgressAction = progressActions
              OnStartup = defaultOnStartup factory
              RunCharacterScripts = defaultRunCharacterScripts
              PreTickUpdate = defaultPreTickUpdate
              PostTickUpdate = defaultPostTickUpdate
              State = stateProvider ()
              StorageProvider = storageProvider
              HandleLogEntry = logHandler
              ScriptProvider = scriptProvider }

        ctx |> provideIdleAction |> PluginRegistry.addActionProvider
        PluginRegistry.addActionHandler "Idle" handleIdleAction
        ctx

    let createDefaultCtx: ILoggerFactory -> (unit -> IStoragePreference) -> WorldContext =
        createDefaultWorldContext createDefaultWorldState
