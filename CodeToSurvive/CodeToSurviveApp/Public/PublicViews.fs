namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Security.SecurityModel
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Options

module PublicViews =
    open Giraffe.ViewEngine
    open Giraffe.ViewEngine.Htmx

    let mainPanel (model: LoginModel) =
        let logBtn =
            match model with
            | ActiveLogin _ -> a [ _href "/logout" ] [ button [ _class "loginLogoutBtn" ] [ encodedText "Logout" ] ]
            | _ -> a [ _href "/login" ] [ button [ _class "loginLogoutBtn" ] [ encodedText "Login" ] ]

        div
            [ _class "headPanel" ]
            [ a [ _href "/" ] [ img [ _class "headPanelLogo"; _src "/android-chrome-192x192.png" ] ]
              a [ _href "/scoreboard" ] [ button [ _class "scoreboardBtn" ] [ encodedText "Scoreboard" ] ]
              a
                  [ _href "https://github.com/Flosus/CodeToSurvive/wiki"; _target "_blank" ]
                  [ button [ _class "wikiBtn" ] [ encodedText "Wiki" ] ]
              logBtn ]

    let adminButton = button [] [ encodedText "Admin" ]
    let overviewButton = button [] [ encodedText "Overview" ]

    let panels (model: LoginModel) (content: XmlNode list) =
        let buttons =
            [ match model with
              | ActiveLogin usrModel ->
                  match usrModel.Role with
                  | Admin -> adminButton
                  | User -> ()

                  overviewButton
              | _ -> () ]

        [ div
              []
              [ if buttons.Length > 0 then div [] buttons else ()
                div [ _class "subPanel" ] content ] ]


    let layout (model: LoginModel) (content: XmlNode list) =
        html
            []
            [ head
                  []
                  [ title [] [ encodedText "CodeToSurvive" ]
                    link [ _rel "stylesheet"; _type "text/css"; _href "/main.css" ]
                    script [ _src "/htmx.org@1.9.2.min.js" ] [] ]
              body [] [ div [] [ mainPanel model; div [] (panels model content) ] ] ]

    let indexView (model: LoginModel) =
        [ p [] [ encodedText "Welcome to CodeToSurvive" ] ]

    let logoutView (model: LoginModel) =
        [ p [] [ encodedText "Thank you for playing" ] ]

    let scoreboardView (model: LoginModel) =
        [ p [] [ encodedText "A scoreboard will be placed here" ] ]

    let loginView (model: LoginModel) =
        let invalidLoginNode = p [ _class "error-message" ] [ encodedText "Invalid Login" ]

        let loginForm =
            [ form
                  [ _hxPost "/login"; _hxSwap "outerHTML" ]
                  [ div
                        []
                        [ if model = LoginModel.InvalidLogin then
                              invalidLoginNode
                          else
                              ()
                          p [] [ encodedText "Username" ]
                          input [ _type "text"; _name "username"; _id "username-login-input" ]
                          p [] [ encodedText "Password" ]
                          // Change this to _type "password"
                          input [ _type "text"; _name "password"; _id "password-login-input" ]
                          button [ _id "submit" ] [ encodedText "Login" ] ] ] ]

        loginForm
