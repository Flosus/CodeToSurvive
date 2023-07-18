namespace CodeToSurvive.App.Admin

open Giraffe.ViewEngine

module AdminViews =

    let adminOverviewView () =
        [ p [] [ encodedText "Here will be the admin overview" ] ]
    
