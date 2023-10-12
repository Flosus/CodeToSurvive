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

    /// Returns the currently loaded plugins sorted by dependencies
    let getPlugins () =
        sortedPlugins <-
            if sortedPlugins.IsSome && sortedPlugins.Value.Length = plugins.Count then
                sortedPlugins
            else
                Some(sortPlugins plugins)

        sortedPlugins.Value
