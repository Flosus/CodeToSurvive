namespace CodeToSurvive.Lib.Storage

open System.IO
open System.Runtime.Serialization.Json
open System.Text
open CodeToSurvive.Lib.Core.Tick
open CodeToSurvive.Lib.Storage.StoragePreference

module StorageManagement =

    let storageFileName x =
        $"{x}.json"
    let serialize (state: 'a) : string =
        let serializer = DataContractJsonSerializer(typeof<'a>)
        let stream = new MemoryStream()
        serializer.WriteObject(stream, state)
        let updateData = stream.ToArray()
        Encoding.UTF8.GetString(updateData)

    let deserialize (json: string) : 'a =
        let instance = typedefof<'a>;
        let ms = new MemoryStream(Encoding.Unicode.GetBytes(json))

        let deserializer = DataContractJsonSerializer(instance);
        let deserializedObj = deserializer.ReadObject(ms);
        deserializedObj :?> 'a
        
    let save (storage: IStoragePreference) (state: State) =
        
        ()
        
    let loadNewest (storage: IStoragePreference) : State =
        let stateStorage = storage.StateStorageFolder
        // TODO get newestState
        let loadedJson = ""
        deserialize loadedJson