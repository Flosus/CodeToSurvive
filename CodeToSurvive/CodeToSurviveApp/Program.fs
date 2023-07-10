module CodeToSurviveApp.App

open System
open System.IO
open CodeToSurvive.App
open CodeToSurvive.App.AppSecurity
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

// ---------------------------------
// Models
// ---------------------------------

type Message = { Text: string }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open Giraffe.ViewEngine
    open Giraffe.Htmx

    let layout (content: XmlNode list) =
        html
            []
            [ head
                  []
                  [ title [] [ encodedText "CodeToSurvive" ]
                    link [ _rel "stylesheet"; _type "text/css"; _href "/main.css" ]
                    script [ _src "htmx.org@1.9.2.min.js" ] [] ]
              body [] content ]

    let partial () =
        h1 [] [ encodedText "CodeToSurviveApp" ]

    let index (model: Message) =
        [ partial (); p [] [ encodedText model.Text ] ] |> layout

    let loginView () =
        [ partial ()
          form
              [ attr "hx-post" "/login" ]
              [ div
                    []
                    [ p [] [ encodedText "Username" ]
                      input [ _id "username" ]
                      p [] [ encodedText "Password" ]
                      input [ _id "password" ]
                      button [ _id "submit" ] [ encodedText "Login" ] ] ] ]
        |> layout

// ---------------------------------
// Web app
// ---------------------------------

let indexHandler (name: string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model = { Text = greetings }
    let view = Views.index model
    htmlView view

let loginHandler =
    let view = Views.loginView ()
    htmlView view

let loginRequestHandler = setStatusCode 200 >=> text "Jeay"
let logoutHandler = signOut "Cookie" >=> redirectTo false "/"

let challengeHandler httpFunc httpContext =
    task {
        let! challengeResult = challenge "Cookie" httpFunc httpContext
        let challengeResultValue = challengeResult.Value

        match challengeResultValue.Response.StatusCode with
        | 401 ->
            let redirect = redirectTo false "/login" httpFunc httpContext
            return! redirect
        | _ -> return challengeResult
    }

let webApp =
    choose
        [ GET
          >=> choose
              [ route "/" >=> indexHandler "world"
                route "/login" >=> loginHandler
                route "/logout" >=> logoutHandler
                requiresAuthentication (challengeHandler)
                >=> choose
                    [ route "/admin" >=> indexHandler "world"
                      route "/survive" >=> indexHandler "world" ]

                ]
          POST >=> choose [ route "/login" >=> loginRequestHandler ]
          setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:5000", "https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
    |> ignore

let configureApp (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false -> app.UseGiraffeErrorHandler(errorHandler).UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore
    services.AddScoped<IAuthenticationService, CTSAuthenticationService>() |> ignore
    ()

let configureLogging (builder: ILoggingBuilder) =
    builder.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")

    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                .UseWebRoot(webRoot)
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(configureServices)
                .ConfigureLogging(configureLogging)
            |> ignore)
        .Build()
        .Run()

    0
