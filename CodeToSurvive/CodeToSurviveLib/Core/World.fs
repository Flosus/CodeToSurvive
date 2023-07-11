namespace CodeToSurvive.Lib.Core

open System

module World =
    
    type Tile =
        {
         tileType: string
         canViewOver: bool
         }

    type Chunk =
        { Name: String
          LocationX: int
          LocationY: int
          Description: String
          MainType: String
          NorthType: String
          SouthType: String
          EastType: String
          WestType: String
          // Size 64x64
          Tiles: Tile[][]
          }

    type WorldMap = Chunk[][]
