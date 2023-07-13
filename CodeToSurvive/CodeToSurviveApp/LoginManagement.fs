namespace CodeToSurvive.App

open System
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

    let loginFileLock = obj ()

    let parseAccountEntry (str: string) =
        let parts = str.Split "|"
        let userId = Guid.Parse parts[0]
        let lastLogin = DateTime.Parse parts[3]

        let role =
            match parts[2] with
            | "Admin" -> AccountRole.Admin
            | _ -> AccountRole.User

        { UserId = userId
          Password = parts[1]
          Role = role
          LastLoginTime = lastLogin
          Username = parts[5] }

    let updatePassword (id: Guid) (newPassword: string) (storage: Storage) : AccountResult =
        let passwordHash = BCrypt.HashPassword(newPassword)
        // TODO read file
        // TODO replace user entry
        // TODO write file
        Error("Not yet implemented")

    let private login (username: string) (password: string) (storage: Storage) : AccountResult =
        // TODO read file
        // Error("Unavailable")
        // TODO find user in file
        // Error("Not logged in")
        // TODO check password
        let passwordHash = "hashFromFile"

        match BCrypt.Verify(password, passwordHash) with
        | true -> Error("Success")
        // TODO build success
        | false -> Error("Not logged in")


    let createNewUser (username: string) (tempPassword: string) (role: AccountRole) (storage: Storage) : AccountResult =
        if username.Contains("|") then
            Error("")
        else
            printf $"Creating new user '{username}'"
            let newUserGuid = Guid.NewGuid()
            let passwordHash = BCrypt.HashPassword(tempPassword)
            let lastLogin = DateTime.Now.ToLocalTime()
            let fileEntry = $"{newUserGuid}|{passwordHash}|{role}|{lastLogin}|{username}"
            // TODO read file
            // TODO check for existing username
            // TODO add user entry
            // TODO write file
            let newAccount =
                { ID = newUserGuid
                  Username = username
                  Role = role }

            Success(newAccount)
