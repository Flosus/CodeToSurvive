namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Public.PublicModels
open Giraffe.ViewEngine

module PrivateViews =

    let overviewView () =
        [ p [] [ encodedText "Here will be the overview" ] ]

    let privateView (_: PublicModel) = overviewView ()

    let characterView () =
        [ p [] [ encodedText "Here will be the overview" ] ]

    let htmxContent (content: unit -> XmlNode list) : XmlNode = content () |> div []
