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
(*let createdCharacter2 =
    CharacterManager.createCharacter context $"{characterName}2" playerName
let createdCharacter3 =
    CharacterManager.createCharacter context $"{characterName}3" playerName
let createdCharacter4 =
    CharacterManager.createCharacter context $"{characterName}4" playerName
let createdCharacter5 =
    CharacterManager.createCharacter context $"{characterName}5" playerName
let createdCharacter6 =
    CharacterManager.createCharacter context $"{characterName}6" playerName
let createdCharacter7 =
    CharacterManager.createCharacter context $"{characterName}7" playerName
let createdCharacter8 =
    CharacterManager.createCharacter context $"{characterName}8" playerName*)

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
