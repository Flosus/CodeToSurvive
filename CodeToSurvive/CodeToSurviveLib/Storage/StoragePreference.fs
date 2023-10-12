namespace CodeToSurviveLib.Storage

open System.IO

module StoragePreference =

    let private defaultSecurityFile = "security.dat"

    type IStoragePreference =
        abstract BaseFolder: DirectoryInfo
        abstract SecurityFolder: DirectoryInfo
        abstract StateStorageFolder: DirectoryInfo
        abstract PlayerStorageFolder: DirectoryInfo

    type StoragePreference(basePath: string) =
        let baseLoc = DirectoryInfo basePath
        let secLoc = baseLoc.CreateSubdirectory "Security"
        let storageLoc = baseLoc.CreateSubdirectory "Storage"
        let stateLoc = storageLoc.CreateSubdirectory "StateStorage"
        let playerLoc = storageLoc.CreateSubdirectory "PlayerStorage"

        interface IStoragePreference with
            member this.BaseFolder = baseLoc
            member this.PlayerStorageFolder = playerLoc
            member this.StateStorageFolder = stateLoc
            member this.SecurityFolder = secLoc

    let getSecFile (storage: IStoragePreference) =
        let secFile = Path.Combine(storage.SecurityFolder.FullName, defaultSecurityFile)

        if File.Exists secFile then
            ()
        else
            (File.Create secFile).Close()

        secFile
