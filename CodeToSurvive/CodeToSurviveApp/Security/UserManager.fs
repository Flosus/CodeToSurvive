namespace CodeToSurvive.App.Security

open CodeToSurvive.App.Security.SecurityModel
open Microsoft.AspNetCore.Identity

type UserManager(store, hasher) =
    inherit UserManager<ApplicationUser>(store, null, hasher, null, null, null, null, null, null)
