namespace CodeToSurvive.Lib.Core

open System.Collections.Generic

module Item =

    type ItemType = TODO

    type Item(name: string) =
        member this.Name = name
        member this.Type = ""
        member this.DefaultWeight = 0.0
        member this.DefaultStackSize = 1

    type ItemEntity =
        { Item: Item
          Amount: double
          Weight: double
          StackSize: int
          // TODO how to save metadata
          Metadata: Dictionary<string, string> }
