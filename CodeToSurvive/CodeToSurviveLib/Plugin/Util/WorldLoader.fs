namespace CodeToSurvive.Lib.Core.Plugin.Util

open System
open System.Runtime.Serialization
open System.Xml
open System.Xml.Serialization

module WorldLoader =

    [<CLIMutable>]
    [<XmlRoot("Action")>]
    type ActionDefinition =
        { [<XmlElement>]
          ActionId: string
          [<XmlElement>]
          Description: string
          [<XmlElement>]
          JobName: string
          [<XmlElement>]
          JobHandler: string
          [<XmlElement>]
          CheckHandler: string
          [<XmlAnyElementAttribute>]
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
          OnUnequip: string
          [<XmlElement>]
          OnLoad: string
          [<XmlElement>]
          OnUnload: string
          [<XmlElement>]
          OnSave: string }

    [<CLIMutable>]
    [<XmlRoot("EquipmentInfo")>]
    type ItemEquipmentInfoDefinition =
        { [<XmlElement>]
          Slot: string }

    [<CLIMutable>]
    [<XmlRoot("Item")>]
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
          [<XmlAnyElementAttribute>]
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
          OnMapStay: string
          [<XmlElement>]
          OnMapSave: string
          [<XmlElement>]
          OnMapLoad: string
          [<XmlElement>]
          OnMapUnload: string }

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
    [<XmlRoot("Map")>]
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
          [<XmlAnyElementAttribute>]
          State: XmlElement }
