namespace CodeToSurvive.Lib.Core

open CodeToSurvive.Lib.Core.Item

module Entity =
    
    type Intelligence = int
    type Strength = int
    type Agility = int
    
    type Stats =
        | Intelligence
        | Strength
        | Agility

    type EntityStats =
        { Intelligence: Intelligence
          Strength: Strength
          Agility: Agility }

    type EntityEquipment =
        { Chest: ItemEntity
          Head: ItemEntity
          Shoes: ItemEntity
          // LeftArm + RightArm OR Arms
          LeftArm: ItemEntity
          RightArm: ItemEntity
          Arms: ItemEntity
          // LeftLeg + RightLeg OR Legs
          LeftLeg: ItemEntity
          RightLeg: ItemEntity
          Legs: ItemEntity
          Back: ItemEntity
          LeftHand: ItemEntity
          RightHand: ItemEntity
          // Rings? Amulet? Glasses?
          Other: ItemEntity }

