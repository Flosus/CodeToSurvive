namespace CodeToSurvive.App

open System
open System.IO
open CodeToSurvive.Lib.Storage
open BCrypt.Net

module LoginManagement =

    type AccountRole =
        | User
        | Admin

    type Account =
        { ID: Guid
          Username: string
          Role: AccountRole }

    type AccountResult =
        | Success of Account
        | Error of string

    type AccountStorageEntry =
        { UserId: Guid
          Password: string
          Role: AccountRole
          LastLoginTime: DateTime
          Username: string }

    let private defaultAdminName = "admin"
    let private randomAdminPassword = "admin" // Guid.NewGuid().ToString()

    let loginFileLock = obj ()

    let parseAccountEntry (str: string) =
        let parts = str.Split "|"
        let userId = Guid.Parse parts[0]
        let passwordHash = parts[1]
        let role = parts[2]
        let lastLogin = DateTime.Parse parts[3]
        let username = parts[4]

        let role =
            match role with
            | "Admin" -> AccountRole.Admin
            | _ -> AccountRole.User

        { UserId = userId
          Password = passwordHash
          Role = role
          LastLoginTime = lastLogin
          Username = username }

    let loadAllUsers storage =
        lock loginFileLock (fun () ->
            let secFile = getSecFile storage
            let lines = File.ReadAllLines secFile

            lines
            |> Array.filter (fun line -> line.Trim().Length <> 0)
            |> Array.map parseAccountEntry)

    let updateUserFile storage entries =
        lock loginFileLock (fun () ->
            (let secFile = getSecFile storage

             entries
             |> Array.map (fun ent -> $"{ent.UserId}|{ent.Password}|{ent.Role}|{ent.LastLoginTime}|{ent.Username}")
             |> (fun entArr -> File.WriteAllLines(secFile, entArr))))


    let updatePassword storage id newPassword =
        let passwordHash = BCrypt.HashPassword(newPassword)

        let mapUsers =
            fun usr ->
                match usr.UserId = id with
                | true -> { usr with Password = passwordHash }
                | false -> usr

        loadAllUsers storage |> Array.map mapUsers |> updateUserFile storage

    let mapAccount entry =
        { ID = entry.UserId
          Username = entry.Username
          Role = entry.Role }

    let login storage username password : AccountResult =
        let allUsers = loadAllUsers storage

        let userIndex =
            allUsers
            |> Array.tryFind (fun ent -> ent.Username = username && BCrypt.Verify(password, ent.Password))

        match userIndex with
        | None -> Error("Not logged in")
        | Some usr -> Success(mapAccount usr)


    let createNewUser storage (username: string) tempPassword role =
        if username.Contains("|") then
            // TODO escape | instead?
            Error("Invalid Username. | is not allowed")
        else
            printf $"Creating new user '{username}'"

            let newUser =
                { UserId = Guid.NewGuid()
                  Password = BCrypt.HashPassword(tempPassword)
                  Role = role
                  LastLoginTime = DateTime.UnixEpoch.ToLocalTime()
                  Username = username }

            loadAllUsers storage |> Array.append [| newUser |] |> updateUserFile storage

            let newAccount = mapAccount newUser

            Success(newAccount)

    let ensureDefaultAdminUser storage =
        let adminNotFound =
            loadAllUsers storage
            |> Array.filter (fun en -> en.Username = defaultAdminName)
            |> Array.isEmpty

        match adminNotFound with
        | true ->
            createNewUser storage defaultAdminName randomAdminPassword AccountRole.Admin
            |> ignore
        | false -> ()
