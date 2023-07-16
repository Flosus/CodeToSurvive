namespace CodeToSurvive.App.Admin

open CodeToSurvive.App.Public.PublicModels
open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx

module AdminViews =

    let adminOverviewView () =
        [ p [] [ encodedText "Here will be the admin overview" ] ]
    
