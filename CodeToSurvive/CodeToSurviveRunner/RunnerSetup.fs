namespace CodeToSurviveRunner

open CodeToSurviveLib
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Action
open CodeToSurviveLib.Storage
open CodeToSurviveLib.Storage.StoragePreference
open CodeToSurviveLib.Util
open CodeToSurviveResource
open CodeToSurviveResource.BasePlugin
open CodeToSurviveResource.DebugPlugin
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
            |> ignore)

    let setupContext (factory: ILoggerFactory) =
        let defaultCtx = WorldContextDefaults.createDefaultCtx factory

        let log = defaultCtx.CreateLogger "Runner"

        let doJobProgress: CharacterAction * WorldContext -> WorldContext =
            fun (_, ctx) ->
                log.LogTrace "doJobProgress"
                ctx

        let runCharacterScripts (ori: WorldContext -> WorldContext) : WorldContext -> WorldContext =
            fun ctx ->
                log.LogTrace "runCharacterScripts"
                ori ctx

        let preTickUpdate (ori: WorldContext -> WorldContext) : WorldContext -> WorldContext =
            fun ctx ->
                log.LogTrace "preTickUpdate"
                ori ctx

        let postTickUpdate (ori: WorldContext -> WorldContext) : WorldContext -> WorldContext =
            fun ctx ->
                log.LogTrace "postTickUpdate"
                ori ctx

        let context: WorldContext =
            { defaultCtx with
                ProgressAction = doJobProgress
                RunCharacterScripts = runCharacterScripts defaultCtx.RunCharacterScripts
                PreTickUpdate = preTickUpdate defaultCtx.PreTickUpdate
                PostTickUpdate = postTickUpdate defaultCtx.PostTickUpdate }

        context

    let setupPlugins () =
        printfn "Setup Plugins"
        BasePlugin.register ()
        DebugPlugin.register ()

    let getStateCallback (storage: StoragePreference) (logProvider: string -> ILogger) =
        let log = logProvider "Runner"
        let statisticsLogger = logProvider "Statistics"

        fun (subCtx: WorldContext) ->
            match subCtx.State.Timestamp.Second with
            | 0 ->
                Statistics.printReport statisticsLogger
                log.LogInformation "Creating backup"
                StorageManagement.save storage subCtx.State
            | _ -> log.LogTrace "Skipping backup"
