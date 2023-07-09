namespace CodeToSurvive.Lib.Core

open System

module World =

    type Chunk =
        { Name: String
          LocationX: int
          LocationY: int
          Description: String
          MainType: String
          NorthType: String
          SouthType: String
          EastType: String
          WestType: String }

    type WorldMap = Chunk[][]
