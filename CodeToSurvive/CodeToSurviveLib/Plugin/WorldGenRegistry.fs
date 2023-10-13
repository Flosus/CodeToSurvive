namespace CodeToSurviveLib.Core.Plugin

open System.Collections.Generic
open CodeToSurviveLib.Core.GameState
open CodeToSurviveLib.Core.Plugin.PluginApi
open CodeToSurviveLib.Core.World

module WorldGenRegistry =

    //___________________________
    // chunk generation
    //___________________________

    exception NoChunkGeneratorFound of string
