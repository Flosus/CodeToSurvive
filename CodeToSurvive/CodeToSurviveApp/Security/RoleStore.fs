namespace CodeToSurviveApp.Security

open System.Threading.Tasks
open Microsoft.AspNetCore.Identity

type RoleStore() =
    interface IRoleStore<IdentityRole> with
        member this.CreateAsync(_, _) = Task.FromResult(IdentityResult.Success)
        member this.DeleteAsync(_, _) = Task.FromResult(IdentityResult.Success)
        member this.Dispose() = ()
        member this.FindByIdAsync(_, _) = Task.FromResult(IdentityRole())
        member this.FindByNameAsync(_, _) = Task.FromResult(IdentityRole())
        member this.GetNormalizedRoleNameAsync(_, _) = Task.FromResult("")
        member this.GetRoleIdAsync(_, _) = Task.FromResult("")
        member this.GetRoleNameAsync(_, _) = Task.FromResult("")

        member this.SetNormalizedRoleNameAsync(_, _, _) =
            Task.FromResult(IdentityRole())

        member this.SetRoleNameAsync(_, _, _) = Task.FromResult(IdentityRole())
        member this.UpdateAsync(_, _) = Task.FromResult(IdentityResult.Success)
