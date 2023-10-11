open System
open System.IO
open System.Runtime.Serialization.Json
open System.Text
open System.Xml
open CodeToSurvive.Lib
open CodeToSurvive.Lib.Core.GameState
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Plugin.Util.WorldLoader
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.Storage
open CodeToSurvive.Lib.Storage.StoragePreference
open CodeToSurvive.Resource
open Microsoft.Extensions.Logging
open System.Xml.Serialization



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
      PostTickUpdate = postTickUpdate
      State = state }

let serializer = DataContractJsonSerializer(typeof<WorldState>)

let stateCallback (ctx: WorldContext) =
    let log = loggerFactory "stateCallback"
    Statistics.printReport (loggerFactory "Statistics")

    match ctx.State.Timestamp.Second with
    | 0 ->
        log.LogInformation "Creating backup"
        StorageManagement.save storage state
    | _ -> log.LogTrace "Skipping backup"

let shouldStop () = isSingleTick

printfn "Setup Plugins"

BasePlugin.register ()
DebugPlugin.register ()

printfn "Run"

let skipTimer = false

GameLoop.gameLoop context stateCallback shouldStop skipTimer |> (fun res -> printfn $"Finished gameLoop with ${res}")
printfn "Finished"

//////


let parseXml (xml: string) (instance: Type) =
    let ms = new MemoryStream(Encoding.UTF8.GetBytes(xml))
    let deserializer = XmlSerializer(instance)
    use xmlReader = XmlReader.Create ms
    let deserializedObj = deserializer.Deserialize(xmlReader)
    deserializedObj

let xmlData =
    File.ReadAllText("..\\..\\documentation\\Development\\Examples\\ActionExample.xml", Encoding.UTF8)

let res = parseXml xmlData typedefof<ActionDefinition> :?> ActionDefinition

let xmlDataItem =
    File.ReadAllText("..\\..\\documentation\\Development\\Examples\\ItemExample.xml", Encoding.UTF8)

let resItem = parseXml xmlDataItem typedefof<ItemDefinition> :?> ItemDefinition

let xmlDataMap =
    File.ReadAllText("..\\..\\documentation\\Development\\Examples\\MapExample.xml", Encoding.UTF8)

let resMap = parseXml xmlDataMap typedefof<MapDefinition> :?> MapDefinition


printfn $"res ${res}"
printfn $"resItem ${resItem}"
printfn $"resItem ${resMap}"
