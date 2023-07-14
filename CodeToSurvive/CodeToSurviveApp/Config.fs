namespace CodeToSurvive.App

open System.IO
open Microsoft.Extensions.Configuration

module Config =

    let settingsFile = "settings.json"

    let configuration =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(settingsFile)
            .Build()

    let getBasePath () = configuration["StoragePath"]
