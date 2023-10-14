namespace CodeToSurviveLib.Core

open System
open System.Runtime.Serialization
open CodeToSurviveLib.Core.Character

module CharacterAction =

    type ActionName = string

    [<DataContract>]
    type Action =
        { [<DataMember>]
          ActionId: String
          [<DataMember>]
          CharacterId: CharacterId
          [<DataMember>]
          Name: ActionName
          [<DataMember>]
          Duration: int
          [<DataMember>]
          mutable CurrentProgress: int
          [<DataMember>]
          mutable IsFinished: bool
          [<DataMember>]
          IsCancelable: bool }

    let getIdleAction charId =
        { ActionId = Guid.Empty.ToString()
          CharacterId = charId
          Name = "Idle"
          Duration = 5
          CurrentProgress = 0
          IsFinished = false
          IsCancelable = false }

    let IsActionFinished (action: Action) =
        TimeSpan.FromSeconds(action.Duration)
        - TimeSpan.FromSeconds(action.CurrentProgress)
        <= TimeSpan.Zero

    let isPlayerActionOpen = fun pAction -> IsActionFinished pAction |> not
