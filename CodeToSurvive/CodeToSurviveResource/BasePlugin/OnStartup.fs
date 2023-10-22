namespace CodeToSurviveResource.BasePlugin

open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin
open CodeToSurviveResource.BasePlugin.Constants
open CodeToSurviveResource.BasePlugin.Actions.GeneralActions
open Microsoft.Extensions.Logging

module OnStartup =

    let registerActions ctx =
        ctx |> provideDrinkAction |> PluginRegistry.addActionProvider 
        PluginRegistry.addActionHandler ("Drink", [| POI |]) handleDrinkAction
        PluginRegistry.addActionHandler ("Drink", [| Item |]) handleDrinkAction
        PluginRegistry.addActionHandler ("Eat", [| Item |]) handleEatAction
        PluginRegistry.addActionHandler ("Walk", [| Transition |]) handleWalkAction


    let onStartup (ctx: WorldContext) : WorldContext =
        let log = ctx.CreateLogger $"{pluginName}.OnStartup"
        log.LogInformation "Did something"
        registerActions ctx
        ctx
