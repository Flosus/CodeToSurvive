open System.IO
open System.Runtime.Serialization.Json
open System.Text
open CodeToSurvive.Lib
open CodeToSurvive.Lib.Core
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Position
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.DevImpl
open CodeToSurvive.Lib.Storage.StoragePreference



printfn "Setting up"
let isSingleTick = true
let storagePath = "../CodeToSurviveApp/CodeToSurviveStorage"
let storage = StoragePreference(storagePath)

let doJobProgress: PlayerTask * State -> State =
    fun (task, state) ->
        printfn "doJobProgress"
        state

let runCharacterScripts: State -> State =
    fun (state) ->
        printfn "runCharacterScripts"
        state

let updateWorldMap: WorldMap -> ChunkPosition -> WorldMap =
    fun map position ->
        printfn "updateWorldMap"
        let gen = getGenerator DevWorldGen.generateChunkDefault
        gen map position

let preTickUpdate: State -> State =
    fun (state) ->
        printfn "preTickUpdate"
        state

let postTickUpdate: State -> State =
    fun (state) ->
        printfn "postTickUpdate"
        state

let state: State =
    { Players = [||]
      Tasks = [||]
      Map =
        { Chunks = ResizeArray()
          SpecialChunks = ResizeArray() } }

let context: WorldContext =
    { ProgressJob = doJobProgress
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

let stateCallback state =
    let stream = new MemoryStream()
    let data = serializer.WriteObject(stream, state)
    let updateData = stream.ToArray()
    let json = (Encoding.UTF8.GetString(updateData))
    printfn $"provideCurrentState called {state}"

let shouldStop () = isSingleTick

printfn "Run"

GameLoop.gameLoop state context stateCallback shouldStop

printfn "Finished"
