namespace CodeToSurvive.Lib

open System.IO

module Storage =

    let defaultSecurityFile = "security.dat"

    type IStorage =
        abstract BaseFolder: DirectoryInfo
        abstract SecurityFolder: DirectoryInfo
        abstract StateStorageFolder: DirectoryInfo
        abstract PlayerStorageFolder: DirectoryInfo

    type Storage(basePath: string) =
        let baseLoc = DirectoryInfo basePath
        let secLoc = baseLoc.CreateSubdirectory "Security"
        let storageLoc = baseLoc.CreateSubdirectory "Storage"
        let stateLoc = storageLoc.CreateSubdirectory "StateStorage"
        let playerLoc = storageLoc.CreateSubdirectory "PlayerStorage"

        interface IStorage with
            member this.BaseFolder = baseLoc
            member this.PlayerStorageFolder = playerLoc
            member this.StateStorageFolder = stateLoc
            member this.SecurityFolder = secLoc


    let getSecFile (storage: IStorage) =
        let secFile = Path.Combine(storage.SecurityFolder.FullName, defaultSecurityFile)

        if File.Exists secFile then
            ()
        else
            (File.Create secFile).Close()

        secFile
