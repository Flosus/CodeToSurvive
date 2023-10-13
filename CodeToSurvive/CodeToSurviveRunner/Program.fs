open System
open System.Threading
open System.Threading.Tasks
open CodeToSurviveLib
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Player
open CodeToSurviveLib.GameLoop
open CodeToSurviveLib.Storage.StoragePreference
open CodeToSurviveRunner



printfn "Setting up"
let mutable shouldStopState = false
let mutable finalState: RunResult option = None
let shouldStop () = shouldStopState
let skipTimer = false
let storagePath = "./"
let playerName = "admin"
let characterName = "AdminCharacter"
let storage = StoragePreference(storagePath)
let context = RunnerSetup.getLoggerFactory () |> RunnerSetup.setupContext
let stateCallback = RunnerSetup.getStateCallback storage context.CreateLogger
RunnerSetup.setupPlugins ()

printfn "Run"

Task.Run(fun () ->
    gameLoop context stateCallback shouldStop skipTimer
    |> (fun res -> printfn $"Finished gameLoop with {res}"))
|> ignore


PlayerManager.addPlayer playerName

let createdCharacter =
    CharacterManager.createCharacter context characterName playerName

let rec cliHandler () =
    let input = Console.ReadLine()

    match input with
    | "exit" ->
        shouldStopState <- true
        printfn "Waiting for exit"

        while finalState.IsNone do
            Thread.Sleep(200)

        printfn $"Finished with {finalState.Value}"
    | _ -> cliHandler ()

cliHandler ()
