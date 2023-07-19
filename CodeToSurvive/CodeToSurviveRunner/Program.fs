open System
open System.Runtime.Serialization.Json
open CodeToSurvive.Lib
open CodeToSurvive.Lib.Core
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Position
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.DevImpl
open CodeToSurvive.Lib.Storage
open CodeToSurvive.Lib.Storage.StoragePreference
open Microsoft.Extensions.Logging



printfn "Setting up"
let isSingleTick = false
let storagePath = "./"
let storage = StoragePreference(storagePath)

let state: State =
    { Timestamp = DateTime.Now
      Players = [||]
      Tasks = [||]
      Map =
        { Chunks = ResizeArray()
          SpecialChunks = ResizeArray() } }

let loggerFactory =
    let factory =
        LoggerFactory.Create(fun builder ->
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter(fun (cat: String) (lvl: LogLevel) ->
                    cat.StartsWith "CodeToSurvive" && lvl >= LogLevel.Debug)
                .AddSimpleConsole(fun opt ->
                    opt.SingleLine <- true
                    opt.IncludeScopes <- true
                    opt.TimestampFormat <- "[yyyy-MM-dd HH:mm:ss]")
                .AddDebug()
            |> ignore)

    fun (name: string) -> factory.CreateLogger $"CodeToSurvive.{name}"

let doJobProgress: PlayerTask * State -> State =
    let log = loggerFactory "doJobProgress"

    fun (_, state) ->
        log.LogTrace "doJobProgress"
        state

let runCharacterScripts: State -> State =
    let log = loggerFactory "runCharacterScripts"

    fun state ->
        log.LogTrace "runCharacterScripts"
        state

let updateWorldMap: WorldMap -> ChunkPosition -> WorldMap =
    let log = loggerFactory "updateWorldMap"

    fun map position ->
        log.LogTrace "updateWorldMap"
        let gen = getGenerator DevWorldGen.generateChunkDefault
        gen map position

let preTickUpdate: State -> State =
    let log = loggerFactory "preTickUpdate"

    fun state ->
        log.LogTrace "preTickUpdate"
        state

let postTickUpdate: State -> State =
    let log = loggerFactory "postTickUpdate"

    fun state ->
        log.LogTrace "postTickUpdate"
        state

let context: WorldContext =
    { CreateLogger = loggerFactory
      ProgressJob = doJobProgress
      RunCharacterScripts = runCharacterScripts
      UpdateWorldMap = updateWorldMap
      PreTickUpdate = preTickUpdate
      PostTickUpdate = postTickUpdate }

state.Map.Chunks.Add
    { Name = "Plains"
      Description = "Empty Plains"
      Location = { X = 0; Y = 0 }
      MainType = WorldGen.plainsType
      NorthType = WorldGen.plainsType
      SouthType = WorldGen.plainsType
      EastType = WorldGen.plainsType
      WestType = WorldGen.plainsType
      Tiles = DevWorldGen.buildPlainTiles () }

let serializer = DataContractJsonSerializer(typeof<State>)

let stateCallback (state: State) =
    let log = loggerFactory "stateCallback"
    Statistics.printReport (loggerFactory "Statistics")

    match state.Timestamp.Second with
    | 0 ->
        log.LogInformation "Creating backup"
        StorageManagement.save storage state
    | _ -> log.LogTrace "Skipping backup"

let shouldStop () = isSingleTick

printfn "Run"

GameLoop.gameLoop state context stateCallback shouldStop |> ignore

printfn "Finished"
