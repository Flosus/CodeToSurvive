namespace CodeToSurviveLib.Core

open System
open CodeToSurviveLib.Core.Domain

module CharacterAction =

    let IsActionFinished (action: CharacterAction) =
        action.CurrentProgress >= action.Duration

    let isPlayerActionOpen = fun pAction -> IsActionFinished pAction |> not

    let updateContext (ctx: WorldContext) (charAction: CharacterAction) =
        ctx.State.ActiveActions <-
            ctx.State.ActiveActions
            |> Array.filter (fun act -> act.ActionId = charAction.ActionId)
            |> Array.append [| charAction |]

        ctx
