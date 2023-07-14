module CodeToSurvive.App.Program

open System
open System.IO
open CodeToSurvive.App.AuthenticationService
open CodeToSurvive.App.Public.PublicHandler
open CodeToSurvive.App.Public.PublicRouter
open CodeToSurvive.App.Private.PrivateRouter
open CodeToSurvive.App.Admin.AdminRouter
open CodeToSurvive.Lib.Storage
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
// Web app
// ---------------------------------

let challengeHandler httpFunc (ctx: HttpContext) =
    let logger = ctx.GetLogger("challengeHandler")

    task {
        let! challengeResult = challenge "Cookie" httpFunc ctx
        let challengeResultValue = challengeResult.Value
        logger.LogTrace $"challengeHandler called {ctx.Request.Path.Value}"

        // TODO fix redirect with HTMX
        match challengeResultValue.Response.StatusCode with
        | 401 ->
            let redirect = redirectTo false "/login" httpFunc ctx
            return! redirect
        | _ -> return challengeResult
    }

// GET-Routes are Public + Authenticated (Private + Admin)
let getRoutes =
    [ requiresAuthentication challengeHandler
      >=> choose (privateGetRoutes |> List.append adminGetRoutes) ]
    |> List.append publicGetRoutesHandler

// POST-Routes are Public + Authenticated (Private + Admin)
let postRoutes =
    [ requiresAuthentication challengeHandler
      >=> choose (privatePostRoutes |> List.append adminPostRoutes) ]
    |> List.append publicPostRoutesHandler

let webApp =
    choose
        [ GET >=> choose getRoutes
          POST >=> choose postRoutes
          setStatusCode 404 >=> text "Not Found" ]

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
    let baseStoragePath = Config.getBasePath ()
    let storage = Storage(baseStoragePath)
    LoginManagement.ensureDefaultAdminUser storage
    let storageFactory (_: IServiceProvider) : obj = storage
    let authService = CTSAuthenticationService(storage)
    let authFactory (_: IServiceProvider) : obj = authService
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore
    services.AddScoped(typedefof<IStorage>, storageFactory) |> ignore
    services.AddScoped(typedefof<IAuthenticationService>, authFactory) |> ignore
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
