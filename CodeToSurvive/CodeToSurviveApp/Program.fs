module CodeToSurvive.App.Program

open System
open System.IO
open CodeToSurvive.App.Security
open CodeToSurvive.App.Security.SecurityModel
open CodeToSurvive.App.Public.PublicHandler
open CodeToSurvive.App.Public.PublicRouter
open CodeToSurvive.App.Private.PrivateRouter
open CodeToSurvive.App.Admin.AdminRouter
open CodeToSurvive.Lib.Storage.StoragePreference
open CodeToSurviveApp.Security
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Identity
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Giraffe


// ---------------------------------
// Web app
// ---------------------------------
let webApp =
    choose
        [ publicRoutes
          privateRoutes
          adminRoutes
          setStatusCode 404 >=> redirectTo false "/404" ]


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
        .UseAuthorization()
        .UseAuthentication()
        .UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore
    services.AddHttpContextAccessor() |> ignore
    // Storage
    let storage = StoragePreference(Config.getBasePath ())
    services.AddSingleton<IStoragePreference>(storage) |> ignore
    // User store
    services.AddIdentity<ApplicationUser, IdentityRole>().AddDefaultTokenProviders()
    |> ignore

    let userStore = new UserStore(storage)
    services.AddSingleton<IUserPasswordStore<ApplicationUser>>(userStore) |> ignore
    let userManager = new UserManager(userStore, PasswordHasher())
    let loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>()
    userManager.Logger <- loggerFactory.CreateLogger(typeof<UserManager<ApplicationUser>>)
    services.AddSingleton<UserManager<ApplicationUser>>(userManager) |> ignore
    services.AddScoped<SignInManager<ApplicationUser>>() |> ignore
    let roleStore = new RoleStore()
    services.AddSingleton<IRoleStore<IdentityRole>>(roleStore) |> ignore

    services.AddAuthorization() |> ignore

    ()

let configureLogging (builder: ILoggingBuilder) =
    builder
        .AddSimpleConsole(fun opt ->
            opt.SingleLine <- true
            opt.IncludeScopes <- true
            opt.TimestampFormat <- "[yyyy-MM-dd HH:mm:ss]")
        .AddDebug()
    |> ignore

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
                .ConfigureLogging(configureLogging)
                .ConfigureServices(configureServices)
            |> ignore)
        .Build()
        .Run()

    0
