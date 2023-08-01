namespace CodeToSurvive.Lib.Core

open System
open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Character
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.Core.Position
open Microsoft.Extensions.Logging

module Tick =

    type CharacterState = { Character: Character }

    type State =
        { Timestamp: DateTime
          Players: CharacterState[]
          Tasks: PlayerTask[]
          Map: WorldMap }

    type RunCharacterScripts = State -> State
    type DoJobProgress = PlayerTask * State -> State
    type StateUpdate = State -> State

    type WorldContext =
        { CreateLogger: string -> ILogger
          ProgressJob: DoJobProgress
          RunCharacterScripts: RunCharacterScripts
          UpdateWorldMap: UpdateWorldMap
          PreTickUpdate: StateUpdate
          PostTickUpdate: StateUpdate }

    let getLogger (factory: ILoggerFactory) category : ILogger = factory.CreateLogger category

    let rec doWithStateUpdate (char: CharacterState[]) (state: State) (act: CharacterState * State -> State) : State =
        match char.Length with
        | 0 -> state
        | _ ->
            let currentPlayer = char[0]
            let newState = act (currentPlayer, state)
            let remainingChars = char[1..]
            doWithStateUpdate remainingChars newState act

    let updateMap (players: CharacterState[]) (state: State) (act: UpdateWorldMap) : State =
        let mapUpdate (player: CharacterState, state: State) : State =
            let chunkPosition = player.Character.PlayerPosition
            let newMap = act state.Map chunkPosition
            { state with Map = newMap }

        doWithStateUpdate players state mapUpdate

    let doJobProgress (character: CharacterState[]) (state: State) (act: DoJobProgress) : State =
        let dJP (cha: CharacterState, state: State) : State =
            let findPlayerTask = fun (cur: PlayerTask) -> cha.Character.Id = cur.Character.Id
            let currentTask = state.Tasks |> Array.find findPlayerTask
            act (currentTask, state)

        doWithStateUpdate character state dJP

    let getPlayersWithoutJob (state: State) =
        let filterPlayerHasNoJob (entry: CharacterState) =
            state.Tasks
            |> Array.filter (fun pt -> pt.Character = entry.Character && not pt.Job.IsCancelable)
            |> Array.isEmpty

        state.Players |> Array.filter filterPlayerHasNoJob

    let tick (state: State) (context: WorldContext) : State =
        let log = context.CreateLogger("tick")
        log.LogDebug "Tick begin"

        let progressJobs (curState: State) : State =
            doJobProgress curState.Players curState context.ProgressJob

        let removeFinishedJobs (curState: State) : State =
            { curState with
                Tasks = curState.Tasks |> Array.filter isPlayerTaskOpen }

        let generateNewMapChunks (curState: State) : State =
            updateMap curState.Players curState context.UpdateWorldMap

        let logStep msg localState =
            log.LogTrace msg
            localState


        state
        // Pre tick work
        |> context.PreTickUpdate
        |> logStep "PreTickUpdate finished"
        // Run player scripts on what to do
        |> context.RunCharacterScripts
        |> logStep "RunCharacterScripts finished"
        // Execute the Jobs the player want to do
        |> progressJobs
        |> logStep "progressJobs finished"
        // Find not finished tasks
        |> removeFinishedJobs
        |> logStep "removeFinishedJobs finished"
        // Generate new terrain
        |> generateNewMapChunks
        |> logStep "generateNewMapChunks finished"
        // Post tick work
        |> context.PostTickUpdate
        |> logStep "PostTickUpdate finished"
        |> (fun state -> { state with Timestamp = DateTime.Now })
        |> logStep "Tick finished"
