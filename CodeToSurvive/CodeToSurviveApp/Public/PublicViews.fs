namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Public.PublicModel

module PublicViews =
    open Giraffe.ViewEngine

    let header (model: LoginModel) =
        let logBtn =
            match model.IsLoggedIn with
            | true -> button [] [ encodedText "Logout" ]
            | false -> button [] [ encodedText "Login" ]

        div [] [ img [ _src "favicon.ico" ]; button [] [ encodedText "Wiki" ]; logBtn ]

    let layout (model: LoginModel) (content: XmlNode list) =
        html
            []
            [ head
                  []
                  [ title [] [ encodedText "CodeToSurvive" ]
                    link [ _rel "stylesheet"; _type "text/css"; _href "/main.css" ]
                    script [ _src "htmx.org@1.9.2.min.js" ] [] ]
              body [] [ div [] [ header (model); div [] content ] ] ]

    let index (model: LoginModel) = [ p [] [] ] |> layout model

    let loginView (model: LoginModel) =
        [ form
              [ attr "hx-post" "/login"; attr "hx-swap" "outerHTML" ]
              [ div
                    []
                    [ p [] [ encodedText "Username" ]
                      input [ _type "text"; _name "username"; _id "username-login-input" ]
                      p [] [ encodedText "Password" ]
                      input [ _type "password"; _name "password"; _id "password-login-input" ]
                      button [ _id "submit" ] [ encodedText "Login" ] ] ] ]
        |> layout model
