namespace CodeToSurviveResource.BasePlugin.Actions

open System
open CodeToSurviveLib.Core
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin.PluginApi
open Microsoft.Extensions.Logging

module GeneralActions =


    let handleDrinkAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    let provideDrinkAction (ctx: WorldContext) : ActionProvider =
        let produce (state, (name, parameter)) =
            let action: CharacterAction =
                { ActionId = Guid.NewGuid()
                  Name = name
                  ActionHandler = name
                  CharacterId = state.Character.Id
                  Duration = 1
                  CurrentProgress = 0
                  IsFinished = false
                  IsCancelable = false
                  Parameter = parameter }

            Some(action)

        produce

    let handleEatAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    let handleWalkAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    ()
