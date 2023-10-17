namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin
open CodeToSurviveResource.BasePlugin.Constants
open Microsoft.Extensions.Logging

module OnStartup =

    let handleDrinkAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    let handleEatAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    let handleWalkAction (ctx: WorldContext) (charAction: CharacterAction) = ctx

    let registerActions =
        PluginRegistry.addAction ("Drink", [| POI |]) handleDrinkAction
        PluginRegistry.addAction ("Drink", [| Item |]) handleDrinkAction
        PluginRegistry.addAction ("Eat", [| Item |]) handleEatAction
        PluginRegistry.addAction ("Walk", [| Transition |]) handleWalkAction
        ()

    let onStartup (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.OnStartup"
        log.LogInformation "Did something"
        ctx
