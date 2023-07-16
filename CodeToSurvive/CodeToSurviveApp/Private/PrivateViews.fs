namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Public.PublicModels
open Giraffe.ViewEngine

module PrivateViews =

    let overviewView (model: PublicModel) =
        [ p [] [ encodedText "Here will be the overview" ] ]
