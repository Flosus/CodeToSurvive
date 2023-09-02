open System
open System.Runtime.Serialization.Json
open CodeToSurvive.Lib
open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.Storage
open CodeToSurvive.Lib.Storage.StoragePreference
open CodeToSurvive.Resource
open Microsoft.Extensions.Logging



printfn "Setting up"
let isSingleTick = false
let storagePath = "./"
let storage = StoragePreference(storagePath)

let state: WorldState =
    { Timestamp = DateTime.Now
      Players = [||]
      Tasks = [||]
      Map = { Chunks = ResizeArray() } }

let factory =
    LoggerFactory.Create(fun builder ->
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter(fun (cat: String) (lvl: LogLevel) -> cat.StartsWith "CodeToSurvive" && lvl >= LogLevel.Debug)
            .AddSimpleConsole(fun opt ->
                opt.SingleLine <- true
                opt.IncludeScopes <- true
                opt.TimestampFormat <- "[yyyy-MM-dd HH:mm:ss]")
            .AddDebug()
        |> ignore)

let loggerFactory =

    fun (name: string) -> factory.CreateLogger $"CodeToSurvive.{name}"

let doJobProgress: PlayerTask * WorldState -> WorldState =
    let log = loggerFactory "doJobProgress"

    fun (_, state) ->
        log.LogTrace "doJobProgress"
        state

let runCharacterScripts: WorldState -> WorldState =
    let log = loggerFactory "runCharacterScripts"

    fun state ->
        log.LogTrace "runCharacterScripts"
        state

let preTickUpdate: WorldState -> WorldState =
    let log = loggerFactory "preTickUpdate"

    fun state ->
        log.LogTrace "preTickUpdate"
        state

let postTickUpdate: WorldState -> WorldState =
    let log = loggerFactory "postTickUpdate"

    fun state ->
        log.LogTrace "postTickUpdate"
        state

let context: WorldContext =
    { CreateLogger = loggerFactory
      ProgressJob = doJobProgress
      RunCharacterScripts = runCharacterScripts
      PreTickUpdate = preTickUpdate
      PostTickUpdate = postTickUpdate }

let serializer = DataContractJsonSerializer(typeof<WorldState>)

let stateCallback (state: WorldState) =
    let log = loggerFactory "stateCallback"
    Statistics.printReport (loggerFactory "Statistics")

    match state.Timestamp.Second with
    | 0 ->
        log.LogInformation "Creating backup"
        StorageManagement.save storage state
    | _ -> log.LogTrace "Skipping backup"

let shouldStop () = isSingleTick

printfn "Setup Plugins"

BasePlugin.register ()
DebugPlugin.register ()

printfn "Run"

GameLoop.gameLoop state context stateCallback shouldStop false |> ignore 
printfn "Finished"
