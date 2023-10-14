open System
open System.Threading
open System.Threading.Tasks
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Player
open CodeToSurviveLib.GameLoop
open CodeToSurviveRunner



printfn "Setting up"
let mutable shouldStopState = false
let mutable finalState: RunResult option = None
let shouldStop () = shouldStopState
let skipTimer = false
let playerName = "admin"
let characterName = "AdminCharacter"
let context = RunnerSetup.getLoggerFactory () |> RunnerSetup.setupContext

let stateCallback =
    RunnerSetup.getStateCallback RunnerSetup.storage context.CreateLogger

RunnerSetup.setupPlugins ()

printfn "Run"

Task.Run(fun () ->
    gameLoop context stateCallback shouldStop skipTimer
    |> (fun res ->
        printfn $"Finished gameLoop with {res}"
        finalState = Some(res)))
|> ignore


PlayerManager.addPlayer playerName

let createdCharacter =
    CharacterManager.createCharacter context characterName playerName

let rec cliHandler () =
    let input = Console.ReadLine()

    match input with
    | "exit" ->
        shouldStopState <- true
    | _ -> cliHandler ()

cliHandler ()
printfn "Waiting for exit"
Thread.Sleep(6000)
printfn $"Finished with {finalState.Value}"
