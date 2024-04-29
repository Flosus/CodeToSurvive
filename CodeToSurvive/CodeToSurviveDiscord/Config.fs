namespace CodeToSurviveDiscord

open FSharp.Configuration


module Config =

    type Settings = AppSettings<"app.config">
