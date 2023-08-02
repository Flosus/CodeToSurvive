namespace CodeToSurvive.Resource.Core

open Microsoft.Extensions.Logging

module Setup =
    
    let setupPlugin (loggerFactory:ILoggerFactory) =
        let logger = loggerFactory.CreateLogger "CodeToSurvive.Resource.Core"
        logger.LogInformation "Setup Core Plugin"
        
        ()
