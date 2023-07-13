namespace CodeToSurvive.Lib.Core

open CodeToSurvive.Lib.Core.Job
open CodeToSurvive.Lib.Core.Character
open CodeToSurvive.Lib.Core.World
open CodeToSurvive.Lib.Core.Position

module Tick =

    type CharacterState = { Character: Character }

    type State =
        { Players: CharacterState[]
          Tasks: PlayerTask[]
          Map: WorldMap }

    type RunCharacterScripts = State -> State
    type DoJobProgress = PlayerTask * State -> State
    type StateUpdate = State -> State

    type WorldContext =
        { ProgressJob: DoJobProgress
          RunCharacterScripts: RunCharacterScripts
          UpdateWorldMap: UpdateWorldMap
          PreTickUpdate: StateUpdate
          PostTickUpdate: StateUpdate }

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
            let chunkPosition = getChunkPosition player.Character.PlayerPosition
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
        // TODO logging?
        // TODO time measurement?
        let progressJobs (curState: State) : State =
            doJobProgress curState.Players curState context.ProgressJob

        let removeFinishedJobs (curState: State) : State =
            { curState with
                Tasks = curState.Tasks |> Array.filter isPlayerTaskOpen }

        let generateNewMapChunks (curState: State) : State =
            updateMap curState.Players curState context.UpdateWorldMap

        state
        // Pre tick work
        |> context.PreTickUpdate
        // Run player scripts on what to do
        |> context.RunCharacterScripts
        // Execute the Jobs the player want to do
        |> progressJobs
        // Find not finished tasks
        |> removeFinishedJobs
        // Generate new terrain
        |> generateNewMapChunks
        // Post tick work
        |> context.PostTickUpdate
