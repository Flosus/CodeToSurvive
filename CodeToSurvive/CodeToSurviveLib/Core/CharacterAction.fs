namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Domain

module CharacterAction =

    let getIdleAction charId =
        { ActionId = Guid.NewGuid()
          CharacterId = charId
          Name = "Idle"
          ActionHandler = "Idle"
          Duration = 1
          CurrentProgress = 0
          IsFinished = false
          IsCancelable = false
          Parameter = None }

    let IsActionFinished (action: CharacterAction) =
        TimeSpan.FromSeconds(action.Duration)
        - TimeSpan.FromSeconds(action.CurrentProgress)
        <= TimeSpan.Zero

    let isPlayerActionOpen = fun pAction -> IsActionFinished pAction |> not
