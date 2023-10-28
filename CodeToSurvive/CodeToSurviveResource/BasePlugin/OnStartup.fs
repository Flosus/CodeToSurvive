namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin
open CodeToSurviveResource.BasePlugin.Constants
open CodeToSurviveResource.BasePlugin.Actions.GeneralActions
open Microsoft.Extensions.Logging

module OnStartup =

    let registerActions ctx =
        ctx |> provideDrinkAction |> PluginRegistry.addActionProvider
        PluginRegistry.addActionHandler "Drink" handleDrinkAction
        // TODO provide drink action
        PluginRegistry.addActionHandler "Eat" handleEatAction
        // TODO provide transition action
        PluginRegistry.addActionHandler "Walk" handleTransitionAction
        // TODO fish action
        // TODO gather
        // TODO fill


    let onStartup (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.OnStartup"
        log.LogInformation "Did something"
        registerActions ctx
        ctx
