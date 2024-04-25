namespace CodeToSurviveDiscord

open System
open System.Collections.Generic
open CodeToSurviveLib.Storage.StoragePreference
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open FSharp.Configuration


module Config =

    type Settings = AppSettings<"app.config">
