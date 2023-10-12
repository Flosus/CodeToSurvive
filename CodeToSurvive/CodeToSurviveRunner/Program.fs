open CodeToSurviveLib
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Storage.StoragePreference
open CodeToSurviveRunner



printfn "Setting up"
let mutable isSingleTick = false
let shouldStop () = isSingleTick
let skipTimer = false
let storagePath = "./"
let storage = StoragePreference(storagePath)
let context = RunnerSetup.getLoggerFactory () |> RunnerSetup.setupContext
let stateCallback = RunnerSetup.getStateCallback storage context.CreateLogger
RunnerSetup.setupPlugins ()

printfn "Run"

GameLoop.gameLoop context stateCallback shouldStop skipTimer
|> (fun res -> printfn $"Finished gameLoop with ${res}")

printfn "Finished"

