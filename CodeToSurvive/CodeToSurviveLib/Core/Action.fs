namespace CodeToSurviveLib.Core

open System
open System.Runtime.Serialization

module Action =

    type ActionName = string

    type CharacterId = Guid

    [<DataContract>]
    type Action =
        { [<DataMember>]
          ID: CharacterId
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

    [<DataContract>]
    type CharacterAction =
        { [<DataMember>]
          CharacterId: CharacterId
          [<DataMember>]
          Action: Action }

    let isPlayerActionOpen = fun pAction -> IsActionFinished pAction.Action |> not
