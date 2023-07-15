namespace CodeToSurvive.App.Public


module PublicModel =
    
    type LoginRequest() = 
        member val username = "" with get, set
        member val password = "" with get, set
    