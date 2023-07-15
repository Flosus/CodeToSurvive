namespace CodeToSurviveApp.Security

open System.Threading.Tasks
open BCrypt.Net
open CodeToSurvive.App.Security.SecurityModel
open Microsoft.AspNetCore.Identity

type RoleStore() =
    interface IRoleStore<IdentityRole> with
        member this.CreateAsync(role, cancellationToken) =
            Task.FromResult(IdentityResult.Success)
        member this.DeleteAsync(role, cancellationToken) =
            Task.FromResult(IdentityResult.Success)
        member this.Dispose() = ()
        member this.FindByIdAsync(roleId, cancellationToken) =
            Task.FromResult(IdentityRole())
        member this.FindByNameAsync(normalizedRoleName, cancellationToken) =
            Task.FromResult(IdentityRole())
        member this.GetNormalizedRoleNameAsync(role, cancellationToken) =
            Task.FromResult("")
        member this.GetRoleIdAsync(role, cancellationToken) = 
            Task.FromResult("")
        member this.GetRoleNameAsync(role, cancellationToken) =
            Task.FromResult("")
        member this.SetNormalizedRoleNameAsync(role, normalizedName, cancellationToken) = 
            Task.FromResult(IdentityRole())
        member this.SetRoleNameAsync(role, roleName, cancellationToken) =
            Task.FromResult(IdentityRole())
        member this.UpdateAsync(role, cancellationToken) =
            Task.FromResult(IdentityResult.Success)