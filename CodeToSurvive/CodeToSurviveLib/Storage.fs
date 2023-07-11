namespace CodeToSurvive.Lib

open System.IO

module Storage =

    type Storage =
        { BaseLocation: DirectoryInfo
          SecurityFolder: DirectoryInfo
          StateStorage: DirectoryInfo
          PlayerStorage: DirectoryInfo }

    let getStorage basePath =
        let baseLoc = DirectoryInfo basePath
        let secLoc = baseLoc.CreateSubdirectory "Security"
        let storageLoc = baseLoc.CreateSubdirectory "Storage"
        let stateLoc = storageLoc.CreateSubdirectory "StateStorage"
        let playerLoc = storageLoc.CreateSubdirectory "PlayerStorage"

        { BaseLocation = baseLoc
          SecurityFolder = secLoc
          StateStorage = stateLoc
          PlayerStorage = playerLoc }
