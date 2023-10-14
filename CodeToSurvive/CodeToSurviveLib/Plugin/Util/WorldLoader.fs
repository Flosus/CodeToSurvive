namespace CodeToSurviveLib.Core.Plugin.Util

open System
open System.IO
open System.Text
open System.Xml
open System.Xml.Serialization
open CodeToSurviveLib.Core.Plugin.PluginApi

module WorldLoader =

    [<CLIMutable>]
    [<XmlRoot("Action")>]
    type ActionDefinition =
        { [<XmlElement>]
          ActionId: string
          [<XmlElement>]
          Description: string
          [<XmlElement>]
          ActionName: string
          [<XmlElement>]
          ActionHandler: string
          [<XmlElement>]
          CheckHandler: string
          [<XmlAnyElement>]
          HandlerParameter: XmlElement }

    [<CLIMutable>]
    [<XmlRoot("Trigger")>]
    type ItemTriggerDefinition =
        { [<XmlElement>]
          OnPickup: string
          [<XmlElement>]
          OnDrop: string
          [<XmlElement>]
          OnEquip: string
          [<XmlElement>]
          OnUnequip: string }

    [<CLIMutable>]
    [<XmlRoot("EquipmentInfo")>]
    type ItemEquipmentInfoDefinition =
        { [<XmlElement>]
          Slot: string }

    [<CLIMutable>]
    [<XmlRoot(ElementName = "Item", Namespace = "https://aheffner.de/Schema")>]
    type ItemDefinition =
        { [<XmlElement>]
          ItemId: string
          [<XmlElement>]
          Name: string
          [<XmlElement>]
          Description: string
          [<XmlElement>]
          Weight: double
          [<XmlElement>]
          Value: int32
          [<XmlElement>]
          Stateful: bool
          [<XmlArray>]
          [<XmlArrayItem("Action")>]
          Actions: ResizeArray<ActionDefinition>
          [<XmlElement>]
          EquipmentInfo: ItemEquipmentInfoDefinition
          [<XmlElement>]
          Trigger: ItemTriggerDefinition
          [<XmlAnyElement>]
          State: XmlElement }



    [<CLIMutable>]
    [<XmlRoot("Transition")>]
    type TransitionDefinition =
        { [<XmlElement>]
          TargetMapId: string
          [<XmlElement>]
          Description: string
          [<XmlElement>]
          DescriptionHandler: string
          [<XmlElement>]
          Check: string }

    [<CLIMutable>]
    [<XmlRoot("Trigger")>]
    type MapTriggerDefinition =
        { [<XmlElement>]
          OnMapEnter: string
          [<XmlElement>]
          OnMapExit: string
          [<XmlElement>]
          OnMapStay: string }

    [<CLIMutable>]
    [<XmlRoot("POI")>]
    type MapPOIDefinition =
        { [<XmlElement>]
          Name: string
          [<XmlElement>]
          Description: string
          [<XmlArray>]
          [<XmlArrayItem("Action")>]
          Actions: ResizeArray<ActionDefinition> }

    [<CLIMutable>]
    [<XmlRoot(ElementName = "Map", Namespace = "https://aheffner.de/Schema")>]
    type MapDefinition =
        { [<XmlElement>]
          MapId: string
          [<XmlElement>]
          Instanced: bool
          [<XmlElement>]
          Persistent: bool
          [<XmlElement>]
          PersistencePool: string
          [<XmlElement>]
          Description: string
          [<XmlArray>]
          [<XmlArrayItem("Transition")>]
          Transitions: ResizeArray<TransitionDefinition>
          [<XmlElement>]
          Trigger: MapTriggerDefinition
          [<XmlArray>]
          [<XmlArrayItem("POI")>]
          POIs: ResizeArray<MapPOIDefinition>
          [<XmlAnyElement>]
          State: XmlElement }


    let private parseXml (xml: string) (instance: Type) =
        let ms = new MemoryStream(Encoding.UTF8.GetBytes(xml))
        let deserializer = XmlSerializer(instance)
        use xmlReader = XmlReader.Create ms
        let deserializedObj = deserializer.Deserialize(xmlReader)
        deserializedObj

    let private load (pluginId: PluginId) (folder: string) =
        let readFile path = File.ReadAllText(path, Encoding.UTF8)

        Directory.EnumerateFiles($"./Data/{pluginId}/{folder}", "*.xml", SearchOption.AllDirectories)
        |> Seq.map readFile
        |> Seq.map (fun xmlString -> parseXml xmlString typedefof<'a> :?> 'a)
        |> Seq.toArray

    let loadMaps (pluginId: PluginId) : MapDefinition[] = load pluginId "Map"
    let loadItems (pluginId: PluginId) : ItemDefinition[] = load pluginId "Map"
