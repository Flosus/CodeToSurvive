namespace CodeToSurviveResource.BasePlugin.Actions

open System
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin.PluginApi

module GeneralActions =


    let handleDrinkAction (ctx: WorldContext) (charAction: CharacterAction) =
        charAction.CurrentProgress <- charAction.CurrentProgress + 1
        ctx

    let provideDrinkAction (ctx: WorldContext) : ActionProvider =
        let produce (state, (name: string, parameter)) =
            match name.ToLower() with
            | "drink" ->
                printfn "providing new drink action"

                Some(
                    { ActionId = Guid.NewGuid()
                      Name = "Drink"
                      ActionHandler = "Drink"
                      CharacterId = state.Character.Id
                      Duration = 1
                      CurrentProgress = 0
                      IsFinished = false
                      IsCancelable = false
                      Parameter = parameter }
                )
            | _ -> None

        produce

    let handleEatAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    let handleWalkAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    ()
