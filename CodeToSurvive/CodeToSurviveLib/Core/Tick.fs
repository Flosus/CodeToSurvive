namespace CodeToSurvive.Lib.Core

open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Player
open CodeToSurvive.Lib.Core.World

module Tick =

    type PlayerState = {
        Player:Player
    }
    
    type State =
        { Players: PlayerState[]
          Tasks: PlayerTask[]
          Map: WorldMap }

    type RunPlayerScripts = State -> State
    type DoJobProgress = PlayerTask * State -> State
    type GenerateChunk = WorldMap -> int -> int -> WorldMap
    type StateUpdate = State -> State

    type WorldContext =
        { ProgressJob: DoJobProgress
          RunPlayerScripts: RunPlayerScripts
          GenerateChunk: GenerateChunk
          PreTickUpdate: StateUpdate
          PostTickUpdate: StateUpdate }

    let rec doWithStateUpdate (players: PlayerState[]) (state: State) (act: PlayerState * State -> State) : State =
        match players.Length with
        | 0 -> newState
        | _ ->
            let currentPlayer = players[0]
            let newState = act (currentPlayer, state)
            let remainingPlayers = players[1..]
            doWithStateUpdate remainingPlayers newState act

    let updateMap (players: PlayerState[]) (state: State) (act: GenerateChunk) : State =
        let mapUpdate (player: PlayerState, state: State) : State =
            let newMap = act state.Map player.Player.WorldMapPositionX player.Player.WorldMapPositionY
            { state with Map = newMap }

        doWithStateUpdate players state mapUpdate

    let doJobProgress (players: PlayerState[]) (state: State) (act: DoJobProgress) : State =
        let dJP (player: PlayerState, state: State) : State =
            let findPlayerTask = fun (cur: PlayerTask) -> player.Player.Id = cur.Player.Id
            let currentTask = state.Tasks |> Array.find findPlayerTask
            act (currentTask, state)

        doWithStateUpdate players state dJP

    let getPlayersWithoutJob (state: State) =
        let filterPlayerHasNoJob (playEntry: PlayerState) =
            state.Tasks
            |> Array.filter (fun pt -> pt.Player = playEntry.Player && not pt.Job.IsCancelable)
            |> Array.isEmpty

        state.Players |> Array.filter filterPlayerHasNoJob

    let tick (state: State) (context: WorldContext) : State =
        // TODO logging?
        // TODO time measurement?
        let progressJobs (curState: State) : State =
            doJobProgress curState.Players curState context.ProgressJob

        let removeFinishedJobs (curState: State) : State =
            { curState with
                Tasks = curState.Tasks |> Array.filter isPlayerTaskOpen }

        let generateNewMapChunks (curState: State) : State =
            updateMap curState.Players curState context.GenerateChunk

        state
        // Pre tick work
        |> context.PreTickUpdate
        // Run player scripts on what to do
        |> context.RunPlayerScripts
        // Execute the Jobs the player want to do
        |> progressJobs
        // Find not finished tasks
        |> removeFinishedJobs
        // Generate new terrain
        |> generateNewMapChunks
        // Post tick work
        |> context.PostTickUpdate
