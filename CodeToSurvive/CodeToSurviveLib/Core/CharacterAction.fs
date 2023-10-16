namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Domain

module CharacterAction =

    let getIdleAction charId =
        { ActionId = Guid.Empty.ToString()
          CharacterId = charId
          Name = "Idle"
          Duration = 5
          CurrentProgress = 0
          IsFinished = false
          IsCancelable = false
          Parameter = [||] }

    let IsActionFinished (action: CharacterAction) =
        TimeSpan.FromSeconds(action.Duration)
        - TimeSpan.FromSeconds(action.CurrentProgress)
        <= TimeSpan.Zero

    let isPlayerActionOpen = fun pAction -> IsActionFinished pAction |> not
