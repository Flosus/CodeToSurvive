namespace CodeToSurviveRunner

open CodeToSurviveLib
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Job
open CodeToSurviveLib.Storage
open CodeToSurviveLib.Storage.StoragePreference
open CodeToSurviveLib.Util
open CodeToSurviveResource
open Microsoft.Extensions.Logging

module RunnerSetup =

    let getLoggerFactory () =
        LoggerFactory.Create(fun builder ->
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter(fun (cat: string) (lvl: LogLevel) ->
                    cat.StartsWith "CodeToSurvive" && lvl >= LogLevel.Debug)
                .AddSimpleConsole(fun opt ->
                    opt.SingleLine <- true
                    opt.IncludeScopes <- true
                    opt.TimestampFormat <- "[yyyy-MM-dd HH:mm:ss]")
                .AddDebug()
            |> ignore)

    let setupContext (factory: ILoggerFactory) =
        let defaultCtx = WorldContextUtil.createDefaultCtx factory

        let doJobProgress: PlayerTask * WorldState -> WorldState =
            let log = defaultCtx.CreateLogger "doJobProgress"

            fun (_, state) ->
                log.LogDebug "doJobProgress"
                state

        let runCharacterScripts: WorldState -> WorldState =
            let log = defaultCtx.CreateLogger "runCharacterScripts"

            fun state ->
                log.LogDebug "runCharacterScripts"
                state

        let preTickUpdate: WorldState -> WorldState =
            let log = defaultCtx.CreateLogger "preTickUpdate"

            fun state ->
                log.LogDebug "preTickUpdate"
                state

        let postTickUpdate: WorldState -> WorldState =
            let log = defaultCtx.CreateLogger "postTickUpdate"

            fun state ->
                log.LogDebug "postTickUpdate"
                state

        let context: WorldContext =
            { defaultCtx with
                ProgressJob = doJobProgress
                RunCharacterScripts = runCharacterScripts
                PreTickUpdate = preTickUpdate
                PostTickUpdate = postTickUpdate }

        context

    let setupPlugins () =
        printfn "Setup Plugins"
        BasePlugin.register ()
        DebugPlugin.register ()

    let getStateCallback (storage: StoragePreference) (logProvider: string -> ILogger) =
        let log = logProvider "stateCallback"
        let statisticsLogger = logProvider "Statistics"

        fun (subCtx: WorldContext) ->
            Statistics.printReport statisticsLogger

            match subCtx.State.Timestamp.Second with
            | 0 ->
                log.LogInformation "Creating backup"
                StorageManagement.save storage subCtx.State
            | _ -> log.LogTrace "Skipping backup"
