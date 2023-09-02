namespace CodeToSurvive.Lib.Core.Plugin

open CodeToSurvive.Lib.Core.Plugin.PluginApi

module PluginRegistry =


    let plugins = ResizeArray<Plugin>()

    // TODO sorting not yet implemented, dependencies do not work
    let mutable private sortedPlugins: Plugin[] option = None

    let registerPlugin plugin = plugins.Add plugin

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
