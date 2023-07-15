namespace CodeToSurvive.App.Security

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open BCrypt.Net
open CodeToSurvive.App.Security.SecurityModel
open CodeToSurvive.Lib.Storage
open Microsoft.AspNetCore.Identity

type UserStore(storage: IStorage) =
    let _storage = storage

    let GetUserFilePath (userId: string) =
        Path.Combine(storage.SecurityFolder.ToString(), userId + ".txt")

    let SerializeUser (user: ApplicationUser) : string =
        $"{user.Id}|{user.PasswordHash}|{user.Role}|{user.UserName}"


    let DeserializeUser (userString: string) : ApplicationUser =
        let parts = userString.Split "|"
        let role = parts[2]
        let username = parts[3].Trim()

        let roleStr =
            match role with
            | "Admin" -> AccountRole.Admin
            | _ -> AccountRole.User

        let usr = ApplicationUser(username, roleStr)
        usr.Id <- parts[0]
        usr.PasswordHash <- parts[1]
        usr.NormalizedUserName <- usr.UserName.ToLower()
        
        usr
    
    
    let defaultAdminName = "admin"
    let randomAdminPassword = "admin" // Guid.NewGuid().ToString()


    
    interface IUserPasswordStore<ApplicationUser> with
        member this.CreateAsync(user: ApplicationUser, _: CancellationToken) =
            File.WriteAllText(GetUserFilePath user.Id, SerializeUser user)
            Task.FromResult(IdentityResult.Success)

        member this.UpdateAsync(user: ApplicationUser, _: CancellationToken) =
            File.WriteAllText(GetUserFilePath user.Id, SerializeUser user)
            Task.FromResult(IdentityResult.Success)

        member this.DeleteAsync(user: ApplicationUser, _: CancellationToken) =
            File.Delete(GetUserFilePath user.Id)
            Task.FromResult(IdentityResult.Success)

        member this.FindByIdAsync(userId: string, _: CancellationToken) =
            let userFilePath = GetUserFilePath userId
            if File.Exists(userFilePath) then
                let userString = File.ReadAllText(userFilePath)
                let user = DeserializeUser userString
                Task.FromResult(user)
            else
                let userString = File.ReadAllText(userFilePath)
                let user = DeserializeUser userString
                Task.FromResult(user)

        member this.FindByNameAsync(normalizedUserName: string, _: CancellationToken) =
            let userFiles = Directory.GetFiles(storage.SecurityFolder.ToString())

            let userFileOption =
                userFiles
                |> Array.tryFind (fun file ->
                    let userString = File.ReadAllText(file)
                    let user = DeserializeUser userString
                    user.NormalizedUserName = normalizedUserName)

            match userFileOption with
            | Some userFile ->
                let userString = File.ReadAllText(userFile)
                let user = DeserializeUser userString
                Task.FromResult(user)
            | None -> Task.FromResult<ApplicationUser>(null)

        member this.Dispose() = ()
        
        // TODO username normalisation?

        member this.GetNormalizedUserNameAsync(user, _) = Task.FromResult(user.UserName.ToLower())
        member this.GetUserIdAsync(user, _) = Task.FromResult(user.Id)
        member this.GetUserNameAsync(user, _) =  Task.FromResult(user.UserName)
        member this.SetNormalizedUserNameAsync(user, normalizedName, _) =
            task {
                user.NormalizedUserName <- normalizedName.ToLower()
            }
        member this.SetUserNameAsync(user, userName, _) = 
            task {
                user.UserName <- userName
            }
        member this.GetPasswordHashAsync(user, _) = Task.FromResult(user.PasswordHash)
        member this.HasPasswordAsync(user, _) = Task.FromResult(user.PasswordHash <> null && user.PasswordHash.Length > 0)
        member this.SetPasswordHashAsync(user, passwordHash, cancellationToken) =
            user.PasswordHash <- passwordHash
            // TODO save password
            Task.CompletedTask

    member this.ensureDefaultAdminUser =
        let dis = this :> IUserPasswordStore<ApplicationUser>
        task {
            let! byName = dis.FindByNameAsync (defaultAdminName, CancellationToken())
            if byName <> null then
                ()
            else
                let password = randomAdminPassword
                let username = defaultAdminName
                printfn "Creating default admin user. Please remember to change the Password."
                printfn $"Username='{username}' Password='{password}'"
                let applicationUser = ApplicationUser(username, Admin)
                applicationUser.Id <- Guid.NewGuid().ToString()
                applicationUser.PasswordHash <- BCrypt.HashPassword randomAdminPassword
                dis.CreateAsync (applicationUser, CancellationToken()) |> ignore
                
        }