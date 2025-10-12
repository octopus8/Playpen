# RTS

## Project Overview
The `RTS` project is a simple real-time strategy (RTS) game implemented using Unity's Entity Component System (ECS). The project demonstrates how to set up a basic RTS game with unit selection, movement, and attacking mechanics using ECS principles.

## ECS
### Basic Concepts
- ECS stands for Entity Component System.
- An Entity is a general purpose object. It is just an ID.
- A Component is a data container that holds data. It does not contain any logic.
- A System contains logic that processes entities with specific components.
- In Unity's ECS, components are structs that implement `IComponentData`.
- Systems can be implemented as `ISystem` (unmanaged) or `SystemBase` (managed).
- Systems use queries to find entities with specific components.
- Some components are "tags" that do not contain any data, but are used to mark entities for specific behavior.

### Component Baking
- Authoring components are MonoBehaviours that are used to add components to entities at bake time.
- Authoring components implement `IBaker` and define a `Bake` method that adds components to the entity.
- The `Baker` class provides methods to add components to the entity, such as `AddComponent`, `AddComponentData`, and `AddSharedComponent`.
- Authoring components are added to GameObjects in the scene or prefabs.
- When the scene is converted to entities, the authoring components are used to add components to the entities.
- Some authoring components may add multiple components to the entity.
- Commonly, the authoring MonoBehavior serializes data that is then passed to the component added to the entity.

## MonoBehaviours
### Singletons
- `BuildingPlacement`
  - Manages the placement of buildings in the game world.
  - Handles user input for selecting and placing buildings.
  - Displays a preview of the building being placed.
  - Ensures that buildings are placed in valid locations.
- `DOTSEvents`
  - Manages event flags for the ECS systems.

### General
- `BuildingBarracksUI`
  - Controls the UI for the barracks building, allowing players to create units and view the spawn queue.
  - The UI includes buttons for creating soldiers and scouts, a progress bar for spawn progress, and a visual representation of the unit queue.
- `BuildingPlacementUI`
  - Controls the UI for building placement.
  - Displays buttons for each building type and highlights the selected building.
  - Subscribes to events from the `BuildingPlacement` singleton to update the UI based on the current building placement state.
- `BuildingPlacementUIButton`
    - Represents a UI button for selecting a building for placement.
    - Controls the button's selection image visibility based on whether it is the currently selected building.
- `CameraController`
    - Manages camera movement, rotation, and zooming.
- `GameOverUI`

## Components / Systems Overview

### General Components
- `FriendlyAuthoring`
  - `Friendly` - Tags the entity as "friendly".
    - This was removed from the project, but could be added back in if needed.
- `EnemyAuthoring`
  - `Enemy`- Tags the entity as "enemy".
  - The `ZombieSpawnerSystem` uses this to control spawning too many enemies close to the spawner.
- `EntityPrefabSetAuthoring`
  - `EntityPrefabSet` - Singleton that contains references to entity prefabs used in the game, such as units and bullets.
    - This is required by the `FindTargetSystem` to find targets, and by the `ShootAttackSystem` to instantiate bullets.
- `EventResetAuthoring`
  - `EventReset` - Singleton that is used to reset event flags at the end of the frame.
    - Systems that set event flags should run before `EventResetSystem`, and systems that read event flags should run after `EventResetSystem`.

### General Systems
- `EventResetSystem` - Resets event flags at the end of the frame.
  - Updates in `LateSimulationSystemGroup`.
  - Note: Does not use job system.


### Units
#### Components
- `UnitAuthoring`
  - `Unit` - Tags the entity as a "unit".
- `UnitSelectedAuthoring`
  - `UnitSelected` - Enableable component containing selection data for an entity.
- `UnitMoverAuthoring`
  - `UnitMover` - Contains movement data such as speed and destination.
- `UnitMoverOverrideAuthoring`
  - `UnitMoverOverride` -  Enableable component containing temporary movement override data for a unit.
- `TargetAuthoring`
  - `Target` - Contains data about the target.
- `FindTargetAuthoring`
  - `FindTarget` - Contains data about finding a target.
- `TargetOverrideAuthoring`
  - `TargetOverride` - Contains temporary target override data for a unit.
    - Overriding the target is used to override auto targeting, such as when a player manually selects a target for a unit to attack.
- `RandomWalkAuthoring`
  - `RandomWalk` - Contains data for random walking behavior.
- `SetupUnitMoverDefaultPositionAuthoring`
  - `SetupUnitMoverDefaultPosition` - Sets the initial position of the `UnitMover` to the entity's current position when added.
- `ShootAttackAuthoring`
  - `ShootAttack` - Contains shoot attack data. This component is added to units that can perform shoot attacks.
- `ShootTargetAuthoring`
  - `ShootTarget` - Contains data about the shoot target.
- `MeleeAttackAuthoring`
  - `MeleeAttack` - Contains melee attack data. This component is added to units that can perform melee attacks.
- `BulletAuthoring`
  - `Bullet` - Contains bullet data such as speed and damage. This component is added to bullet entities.
- `EnemyAttackHQAuthoring`
  - `EnemyAttackHQ` - Tags the entity as an enemy headquarters that can be attacked by enemies.
- `EnemyAuthoring`
  - `Enemy` - Tags the entity as an enemy.
- `EntityPrefabSetAuthoring`
  - `EntityPrefabSet` - Singleton that contains references to entity prefabs used in the game, such as units and bullets.
- `FactionAuthoring`
  - `Faction` - Contains faction data for the entity.
- `FindTargetAuthoring`
  - `FindTarget` - Contains data about finding a target.
- `LoseTargetAuthoring`
  - `LoseTarget` - Contains data about losing a target.
- `HealthAuthoring`
  - `Health` - Contains health data for the entity.
- `HealthBarAuthoring`
  - `HealthBar` - Contains health bar data for the entity.
- `ShootLightAuthoring`
  - `ShootLight` - Contains data for a light effect when shooting.

#### Systems
- `UnitSelectedVisualSystem` - Updates a selectable unit's "selected" visual based on whether it is selected or not.
  - Updates in `LateSimulationSystemGroup` and before `EventResetSystem`.
  - Note: Does not use job system.
- `UnitMoverSystem` - Moves units towards their destination.
- `UnitMoverOverrideSystem` - Sets the `UnitMover` target position to the `UnitMoverOverride` position if it is set.
  - Note: Does not use job system.
- `FindTargetSystem` - Finds targets for entities within a specified range and updates the `Target` component.
  - Note: Does not use job system.
- `ShootAttackSystem` - Handles shooting for entities that can shoot.
  - Requires `EntityPrefabSet` singleton to be present before updating.
  - Note: Does not use job system.
- `MeleeAttackSystem` - Handles melee attacks for entities that can perform melee attacks.
  - Requires `EntityPrefabSet` singleton to be present before updating.
  - Note: Does not use job system.


### Buildings
#### Components
- `BuildingBarracksAuthoring`
  - `BuildingBarracks` - Contains data for spawning units. This component is added to barracks buildings.
    - Contains an `InternalBufferCapacity` attribute to enqueue spawned units before they are instantiated.
  - `SpawnBuffer` - Buffer element for storing units queued to be spawned by the barracks.
  - `BuildingBarracksUnitEnqueue` - Event component used to enqueue a unit to be spawned by the barracks.
    - This component is enabled when a unit is queued for spawning.
- `BuildingFriendlyHQAuthoring`
  - `BuildingFriendlyHQ` - Singleton tag for the friendly headquarters building. 
- `BuildingTypeAuthoring`
  - `BuildingType` - Component which specifies the type of building an entity represents.


#### Systems
- `BuildingBarracksSystem` - Spawns units from barracks buildings at a set interval.
  - Requires `EntityPrefabSet` singleton to be present before updating.
  - Note: Does not use job system.


############################################################################################

- Things to document
  - Basics concepts of ECS
    - The use of structs for components
      - `IComponentData`
    - The difference between ISystem and SystemBase
    - How systems query for entities with specific components
    - The use of tags as components
  - Singleton MonoBehaviours
  - Singleton ECS entities
  - Barracks spawning units
    - InternalBufferCapacity
  - Unit Movement
  - Unit Selection
  - Targeting
  - Shooting
  - Melee Attacks
  - Spawning Enemies

############################################################################################
