namespace CodeToSurviveLib.Core.Plugin

open System
open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin.PluginApi
open Microsoft.Extensions.Logging

module PluginRegistry =
    type PluginProvider = ILoggerFactory -> Plugin

    let plugins = ResizeArray<Plugin>()

    // TODO sorting not yet implemented, dependencies do not work
    let mutable private sortedPlugins: Plugin[] option = None
    let mutable private pluginProviders: PluginProvider[] = [||]

    let registerPlugin (pluginProvider: PluginProvider) =
        pluginProviders <- pluginProviders |> Array.append [| pluginProvider |]

    let private sortPlugins (plugins: ResizeArray<Plugin>) : Plugin[] =
        let _ = ResizeArray<Plugin>()

        plugins.ToArray()

    let registerPlugins (factory: ILoggerFactory) =
        pluginProviders |> Array.map (fun func -> func factory) |> plugins.AddRange


    /// Returns the currently loaded plugins sorted by dependencies
    let getPlugins () =
        sortedPlugins <-
            if sortedPlugins.IsSome && sortedPlugins.Value.Length = plugins.Count then
                sortedPlugins
            else
                Some(sortPlugins plugins)

        sortedPlugins.Value

    (*
    Action Registry
    *)

    let mutable private actionHandlerRegistry: (ActionName * ActionHandler)[] =
        [||]

    let addActionHandler key (handler: ActionHandler) =
        let findBy ent =
            let entKey, _ = ent
            entKey <> key

        actionHandlerRegistry <-
            actionHandlerRegistry
            |> Array.filter findBy
            |> Array.append [| (key, handler) |]

    let findAction (findBy: ActionName) : ActionHandler option =
        let findBy ent =
            let key, _ = ent
            key = findBy

        actionHandlerRegistry |> Array.rev |> Array.tryFind findBy |> Option.map snd



    let mutable private actionProvider: ActionProvider[] = [||]

    let addActionProvider provider =
        actionProvider <- actionProvider |> Array.append [| provider |]

    let getActionProvider (input: CharacterState * (string * Object[] option)) =
        let rec getAction index =
            match index >= actionProvider.Length with
            | true -> None
            | false ->
                match actionProvider[index]input with
                | Some res -> Some(res)
                | None -> getAction (index + 1)

        getAction 0
