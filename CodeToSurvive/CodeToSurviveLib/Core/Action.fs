namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Character

module Action =

    type ActionName = string

    type Action =
        { ID: Guid
          Name: ActionName
          Duration: int
          mutable CurrentProgress: int
          mutable IsFinished: bool
          IsCancelable: bool }

    let IdleAction =
        { ID = Guid.Empty
          Name = "Idle"
          Duration = 5
          CurrentProgress = 0
          IsFinished = false
          IsCancelable = false }

    let IsActionFinished (action: Action) =
        TimeSpan.FromSeconds(action.Duration)
        - TimeSpan.FromSeconds(action.CurrentProgress)
        <= TimeSpan.Zero

    type CharacterAction =
        { Character: Character; Action: Action }

    let isPlayerActionOpen = fun pAction -> IsActionFinished pAction.Action |> not
