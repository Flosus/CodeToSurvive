namespace CodeToSurvive.Lib.Core.Plugin.Util

open System
open System.Runtime.Serialization
open System.Xml
open System.Xml.Serialization

module WorldLoader =

    [<CLIMutable>]
    [<DataContract(Name = "Action", Namespace = "")>]
    type ActionDefinition =
        { [<DataMember(Order = 0)>]
          ActionId: string
          [<DataMember(Order = 1)>]
          Description: string
          [<DataMember(Order = 2)>]
          JobName: string
          [<DataMember(Order = 3)>]
          JobHandler: string
          [<DataMember(Order = 4)>]
          CheckHandler: string
          [<DataMember(Order = 5)>]
          HandlerParameter: XmlElement }

    [<CLIMutable>]
    [<DataContract(Name = "Item", Namespace = "")>]
    [<KnownType(typedefof<ActionDefinition>)>]
    type ItemDefinition =
        { [<DataMember(Order = 0)>]
          itemId: string
          [<DataMember(Order = 1)>]
          name: string
          [<DataMember(Order = 2)>]
          Description: string
          [<DataMember(Order = 3)>]
          weight: double
          [<DataMember(Order = 4)>]
          value: int32
          [<DataMember(Order = 5)>]
          stateful: bool
          [<DataMember(Order = 6)>]
          Actions: List<ActionDefinition>

          // [<DataMember(Order = 7)>]
          // equipmentInfo: XmlNode
          //
          // [<DataMember(Order = 8)>]
          // trigger: XmlNode[]
          //
          // [<DataMember(Order = 9)>]
          // state: XmlNode[]

        }

    ()
