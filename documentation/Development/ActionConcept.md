# Action Concept

## Arguments

Actions can have different and optional arguments. Arguments that can be skipped are markt with an `opt`. Actions performed on an item or POI in a script automaticly use themself as an argument.

* `Item` An item in your inventory or an equiped item.
* `POI` A POI on the map.
* `Transition` A transition on the map.
* `Item@POI` An item on the map.
* `Entity` A character or mob on the map.
* `ItemSlot` An itemslot on your character. Use this for equiping or unequiping items.
* `Text` Just a string. This is dependend on the action.

## Actions

Here are some general actions as examples. More actions can be found on items and POIs in the world.

### Walk

#### Parameter

* Transition

#### Time

* 10

### Drink

#### Parameter

* Item|POI|Item@POI

### Eat

#### Parameter

* Item|Item@POI

### Pickup

#### Parameter

* Item@POI

### Drop

Drop an item fron your inventory onto or into a POI.

#### Parameter

* Item
* POI


### Fish

Try to fish in a source of water. The POI might not be valid, e.g. a water fountain.
An item in your inventory can be used to augment the action by reducing the wait time by increasing the catch chance or improving the caught item.

#### Parameter

* POI
* Item opt

#### Time

* 5 - infinite

The character will fish until something is caught or the action is canceled.

### Equip

### Unequip

## Examples

### Drink from a river

```lua
local riverPOI = ...
```

### Eat berries in your inventory

```lua
local inventory = ...
```

### Pick up a stone on the ground

```lua
local poiWithStones = ...
```

### Drop a stone from your inventory

```lua
local inventory = ...
```
