namespace CodeToSurviveLib.Core.Plugin

open CodeToSurviveLib.Core.Plugin.PluginApi

module PluginRegistry =
    open Microsoft.Extensions.Logging

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

    let mutable private actionRegistry: (ActionHandlerKey * ActionHandler)[] = [||]
    
    let addAction key (handler:ActionHandler) =
        let findBy ent =
            let entKey, _ = ent
            entKey <> key
        actionRegistry <- actionRegistry |> Array.filter findBy |> Array.append [|(key, handler)|]
    
    let findAction (findBy: ActionHandlerKey) : ActionHandler option =
        let findBy ent =
            let key, _ = ent
            key = findBy
        actionRegistry |> Array.rev |> Array.tryFind findBy |> Option.map snd