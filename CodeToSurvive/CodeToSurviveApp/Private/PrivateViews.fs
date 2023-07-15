namespace CodeToSurvive.App.Private

open CodeToSurvive.App.Security.SecurityModel
open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx
module PrivateViews =
    
    let overviewView (model: LoginModel) =
        [ p [] [ encodedText "Here will be the overview" ] ]
        
        
