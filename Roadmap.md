# Roadmap

Features and TODOs of CodeToSurvive.

## V 0.1 - The "It works"-Release

The initial release requires the implementation of most internal features.

* Item-Management
    * Type changes
    * Plugin-API changes
* Map-Management
    * Type changes
    * Plugin-API changesl
* Saving/Loading
  * WorldState-Save/Load
  * Plugin-Save/Load
  * Restore Handlers with Load
  * Validate Handlers after Load
* Implement Log-Management for Character-Logs
* Finish Script-API
    * CommunicationApi
        * getLastWords
    * WorldApi
        * getMap
        * getActions
        * getPOIs
        * getItems?
    * CharacterApi
        * getInventory
        * getEquipment
        * getStatus
    * ActionApi
        * getActiveAction
        * createAction
* Finish starting World
    * (/) A few maps
    * POIs on the map (Requires Actions)
* New Items
    * Fish
    * Grilled Fish
    * Waterskin
* New Actions
    * Transition
    * Fish
    * (Berry) Gather
    * Drink
    * FillWithWater
    * Eat
* Write Tests and create a framework for easy testing

# V 0.2 - The "Frontend"-Release

The main target of this release is to provide a browser-frontend experience.

* Implement the HTMX-Interface
    * Admin: User-management
    * Player: Character management (Single)
* World Expansion
    * Finish MapConcept.drawio implementation, without the dungeon
    * Expand Items
        * More food
        * Stone, Metals and Minerals by mining and foraging
        * Wood from the forest
    * Expand Actions
* Documentation
    * Script-Api
    * Script-Examples
    * Game-World

# V 0.3 - The "Parallel"-Release

This release adds multi-char support and preparations for combat.

* Add Multi-character-support in the fronted
* World Expansion
    * Goblin Cave
    * Weapons
    * Armor
    * Healing Items

# V 0.4 - The "Combat"-Release

It's fighting time.

* Add combat v1
* NPCs/Entities
* Documentation
  * HowTo-Combat

# Future Features

* Skills/abilities
    * Random skill/ability unlocks
    * Improve them by using the associated ability
* CLI-Client
* Interactive Mode in browser.


