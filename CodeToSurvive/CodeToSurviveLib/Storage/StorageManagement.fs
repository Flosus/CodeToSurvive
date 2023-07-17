namespace CodeToSurvive.Lib.Storage

open System
open System.IO
open System.Runtime.Serialization.Json
open System.Text
open System.Text.RegularExpressions
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Storage.StoragePreference

module StorageManagement =

    let dateTimeFormat = "yyyy-MM-dd_HH:mm:ss"
    let filePattern = "storage_(\d{4}-\d\d-\d\d_\d\d:\d\d:\d\d).json"
    // storage-yyyy-MM-dd.json
    let storageFileName x = $"storage_{x}.json"

    let serialize (state: 'a) : string =
        let serializer = DataContractJsonSerializer(typeof<'a>)
        let stream = new MemoryStream()
        serializer.WriteObject(stream, state)
        let updateData = stream.ToArray()
        Encoding.UTF8.GetString(updateData)

    let deserialize (json: string) : 'a =
        let instance = typedefof<'a>
        let ms = new MemoryStream(Encoding.Unicode.GetBytes(json))

        let deserializer = DataContractJsonSerializer(instance)
        let deserializedObj = deserializer.ReadObject(ms)
        deserializedObj :?> 'a

    let save (storage: IStoragePreference) (state: State) =
        let fileName = DateTime.Now.ToString dateTimeFormat |> storageFileName
        let content = serialize state
        let savePath = Path.Join(storage.StateStorageFolder.FullName, fileName)
        File.WriteAllText(savePath, content)

    let loadNewest (storage: IStoragePreference) : State =
        storage.StateStorageFolder.GetFiles()
        |> Array.filter (fun dir -> Regex.IsMatch(dir.Name, filePattern))
        |> Array.map (fun dir -> (dir, Regex.Match(dir.Name, filePattern).Value))
        |> Array.maxBy snd
        |> (fun (file, _) -> file.FullName)
        |> File.ReadAllText
        |> deserialize
