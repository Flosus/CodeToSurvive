namespace CodeToSurvive.App.Public

open CodeToSurvive.App.Public.PublicModels
open CodeToSurvive.App.Security.SecurityModel

module PublicViews =
    open Giraffe.ViewEngine
    open Giraffe.ViewEngine.Htmx

    let mainPanel (model: PublicModel) =
        let logBtnLink, logBtnTxt =
            match model.loginModel with
            | ActiveLogin _ -> ("/logout", "Logout") // a [ _href "/logout"; _class "headPanelBtn" ] [ button [ _class "loginLogoutBtn" ] [ encodedText "Logout" ] ]
            | _ -> ("/login", "Login") // a [ _href "/login"; _class "headPanelBtn" ] [ button [ _class "loginLogoutBtn" ] [ encodedText "Login" ] ]

        div
            [ _class "headPanel" ]
            [ a [ _href "/" ] [ img [ _class "headPanelLogo"; _src "/android-chrome-192x192.png" ] ]
              div
                  [ _class "panelMenu" ]
                  [ a
                        [ _href "/secured/private"; _class "panelLink" ]
                        [ button [ _class "panelBtn" ] [ encodedText "Survive" ] ]
                    a
                        [ _href "/scoreboard"; _class "panelLink" ]
                        [ button [ _class "panelBtn" ] [ encodedText "Scoreboard" ] ]
                    a
                        [ _href "https://github.com/Flosus/CodeToSurvive/wiki"
                          _target "_blank"
                          _class "panelLink" ]
                        [ button [ _class "panelBtn" ] [ encodedText "Wiki" ] ]
                    a
                        [ _href logBtnLink; _class "panelLink" ]
                        [ button [ _class "panelBtn" ] [ encodedText logBtnTxt ] ] ] ]

    let adminButton =
        a [ _href "/secured/admin"; _class "panelLink" ] [ button [ _class "panelBtn" ] [ encodedText "Admin" ] ]

    let overviewButton =
        a
            [ _href "/secured/private/overview"; _class "panelLink" ]
            [ button [ _class "panelBtn" ] [ encodedText "Overview" ] ]

    let panels (model: PublicModel) (content: XmlNode list) =
        let buttons =
            [ match model.loginModel, model.isHtmxRequest with
              | ActiveLogin usrModel, isHtmx when isHtmx ->
                  match usrModel.Role with
                  | Admin -> adminButton
                  | User -> ()

                  overviewButton
              | _ -> () ]

        [ div
              []
              [ if buttons.Length > 0 then
                    div [ _class "panelMenu" ] buttons
                else
                    ()
                div [ _class "subPanel" ] content ] ]


    let layout (model: PublicModel) (content: XmlNode list) =
        html
            []
            [ head
                  []
                  [ title [] [ encodedText "CodeToSurvive" ]
                    link [ _rel "stylesheet"; _type "text/css"; _href "/main.css" ]
                    script [ _src "/htmx.org@1.9.2.min.js" ] [] ]
              body [] [ div [] [ mainPanel model; div [] (panels model content) ] ] ]

    let indexView (_: PublicModel) =
        [ p [] [ encodedText "Welcome to CodeToSurvive" ] ]

    let logoutView (_: PublicModel) =
        [ p [] [ encodedText "Thank you for playing" ] ]

    let scoreboardView (_: PublicModel) =
        [ p [] [ encodedText "A scoreboard will be placed here" ] ]

    let notFoundView (_: PublicModel) = [ p [] [ encodedText "404" ] ]

    let loginView (model: PublicModel) =
        let invalidLoginNode = p [ _class "error-message" ] [ encodedText "Invalid Login" ]

        let loginForm =
            [ form
                  [ _hxPost "/login"; _hxSwap "outerHTML"; _class "loginForm" ]
                  [ div
                        [ _class "loginFormBox" ]
                        [ if model.loginModel = LoginModel.InvalidLogin then
                              invalidLoginNode
                          else
                              ()
                          p [] [ encodedText "Username" ]
                          input [ _type "text"; _name "username"; _id "username-login-input" ]
                          p [] [ encodedText "Password" ]
                          // Change this to _type "password"
                          input [ _type "text"; _name "password"; _id "password-login-input" ]
                          button [ _id "submit"; _class "loginFormSubmitButton" ] [ encodedText "Login" ] ] ] ]

        loginForm
