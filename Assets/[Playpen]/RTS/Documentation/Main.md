# RTS

## Overview
The `RTS` project is a simple real-time strategy (RTS) game implemented using Unity's Entity Component System (ECS). The project demonstrates how to set up a basic RTS game with unit selection, movement, and attacking mechanics using ECS principles.

## ECS Overview
- ECS stands for Entity Component System.
- An Entity is a general purpose object. It is just an ID.
- A Component is a data container that holds data. It does not contain any logic.
- A System contains logic that processes entities with specific components.
- In Unity's ECS, components are structs that implement `IComponentData`.
- Systems can be implemented as `ISystem` (unmanaged) or `SystemBase` (managed).
- Systems use queries to find entities with specific components.
- Some components are "tags" that do not contain any data, but are used to mark entities for specific behavior.

## Component Baking
- Authoring components are MonoBehaviours that are used to add components to entities at bake time.
- Authoring components implement `IBaker` and define a `Bake` method that adds components to the entity.
- The `Baker` class provides methods to add components to the entity, such as `AddComponent`, `AddComponentData`, and `AddSharedComponent`.
- Authoring components are added to GameObjects in the scene or prefabs.
- When the scene is converted to entities, the authoring components are used to add components to the entities.
- Some authoring components may add multiple components to the entity.
- Commonly, the authoring MonoBehavior serializes data that is then passed to the component added to the entity.


## Script Overview

### General Components
- `Friendly`(`FriendlyAuthoring`) - Tags the entity as "friendly".
  - This was removed from the project, but could be added back in if needed.
- `Enemy`(`EnemyAuthoring`) - Tags the entity as "enemy". The `ZombieSpawnerSystem` uses this to control spawning too many enemies close to the spawner.
- `Faction`(`FactionAuthoring`) - Defines which faction the entity belongs to. The `FindTargetSystem` uses this to find enemy targets, and `UnitSelection` uses this to find selected units.


### Health
#### Components
- `Health`(`HealthAuthoring`) - Contains health information such as current health and max health, and a set of event flags, such as "on health changed" and "on dead".
- `HealthBar`(`HealthBarAuthoring`) - Contains data for displaying a health bar above the entity.
#### Systems
- `HealthBarSystem` - Updates the health bar visual.
- `HealthDeadTestSystem` - Tests the health component for death and removes the entity if dead.

----------------------
(verify the documentation below is accurate)

### Units
#### Components
- `Unit`(`UnitAuthoring`) - Tags the entity as a "unit".
- `UnitMover`(`UnitMoverAuthoring`) - Contains movement data such as speed and destination.
- `UnitMoverOverride`(`UnitMoverOverrideAuthoring`) -  Enableable component containing temporary movement override data for a unit.
- `Selected`(`SelectedAuthoring`) - Enableable component containing selection data for an entity.
- `Target`(`TargetAuthoring`) - Tags the entity as a target and contains target data.

- `FindTarget`(`FindTargetAuthoring`) - Tags the entity as needing to find a target and contains search data.
- `ShootAttack`(`ShootAttackAuthoring`) - Tags the entity as being able to shoot and contains shooting data.
- `EnemyAttackHQ`(`EnemyAttackHQAuthoring`) - Tags the entity as an enemy that will attack the HQ.
#### Systems
- `UnitMoverSystem` - Moves units towards their destination.
- `UnitMoverOverrideSystem` - Sets the `UnitMover` target position to the `UnitMoverOverride` position if it is set.
- `SelectedVisualSystem` - Updates a selectable unit's "selected" visual based on whether it is selected or not.
  - Updates in `LateSimulationSystemGroup` and before `EventResetSystem`.

- `FindTargetSystem` - Finds targets for entities that need to find a target.
- `ShootAttackSystem` - Handles shooting for entities that can shoot.
  - Uses the `EntityReferences` to instantiate bullets.

----------------------

## Animation Data Baking (Custom Baking System)
In order to bake the mesh data for animations, the meshes must exist on entities in the scene at bake time. The `FlipbookAnimationDataHolderAuthoring` component creates temporary bake-time only entities that contain the animation meshes. `FlipbookAnimationDataHolderBakingSystem` is a custom baking system.  At bake time, this system then builds the animation data that is used during runtime. In the end, the temporary bake-time only entities are desroyed after baking, but the mesh data reamins baked.

----------------------


### Unit Components
- Unit - Tags the entity as a "unit".
- Unit Mover:Target Position
  - EnemyAttackHQSystem: If the unit does not already have a target, the UnitMover target position is set to the HQ position.
  - UnitMoverSystem: Uses the UnitMover target position to move the unit towards the target position.
  - MeleeAttackSystem: Sets the UnitMover target position to a melee target.
  - RandomWalkingSystem: Sets the UnitMover target position to a random position.
  - UnitMoverOverrideSystem: Sets the UnitMover target position to the move override position.
- Unit Mover Override:Target Position
  - BuildingBarracksSystem: After instantiating a unit, the UnitMoverOverride target position is set to the right of the barracks. THIS HAS TO USE THE OVERRIDE. WHY IS THAT?
  - UnitSelection: When clicking the destination for selected units, the UnitMoverOverride target position is set to the destination.
  - UnitMoverOverrideSystem: Sets the UnitMover target position to the move override position.

  

----------------------------



## Scripts
- All scripts for this project are in the `Playpen\Assets\[Playpen]\Scripts` folder.
- At the root, the `Scripts` directory contains scripts that contain general app data structures.

- `Scripts\Systems` - Contains scripts that define systems used in ECS.
  - `FlipbookAnimation`
    - `ActiveFlipbookAnimationSystem`
      - Sets the mesh of the entity based on its current active animation and frame.
      - Advances the frame based on the frame duration and deltaTime.
      - If the animation is a one-shot (like shooting or melee attack) and has completed, resets to None.
    - `ChangeFlipbookAnimationJob`
      - Changes the active animation if the next animation is different from the current one.
      - If the animation is a one-shot (like shooting or melee attack), it will not be changed until it is finished.
- `Scripts\Components` - Contains scripts that define ECS authoring components.
  - `FlipbookAnimationDataHolderAuthoring`





- `Scripts\MonoBehaviours` - Contains scripts that work with MonoBehaviours.
  - `RTSGame` - Singleton that contains general game data.
  - `MouseWorldPosition` - Singleton used to get the mouse world position through the single exposed method `GetMouseWorldPosition`.
    - This is used by `UnitSelection` to select units.
  - `UnitSelection` - Singleton that handles unit selection input and updates entities appropriately.
    - Demonstrates how to use `EntityCommandBuffer` to modify entities from a MonoBehaviour.
    - Demonstrates getting the screen position for a unit.
    - For multiple selection, uses the `Contains` method of `Rect` to determine if a unit is within the selection box.
    - For single selection, uses a raycast to determine if a unit was clicked on.
    - Exposes events for selection area start and end, allowing other components to respond to these events (e.g., updating the UI).
  - `UnitSelectionUI` - Updates the unit selection UI.
    - Subscribes to events from the `UnitSelection` singleton to manage the selection area UI element.
- `Scripts\Components` - Contains scripts that define ECS authoring components.
  - 
  - `UnitAuthoring` - Marks an entity as a unit and contains data about the unit.
  - `FriendlyAuthoring` - Marks an entity as friendly.
  - `ZombieAuthoring` - Marks an entity as a zombie.
  - `UnitMoverAuthoring` - Marks an entity as movable and contains data about movement.
  - `SelectedAuthoring` - Marks an entity as selectable and contains data about selection.
  - `TargetAuthoring` - Marks an entity as a target and contains data about the target.
  - `FindTargetAuthoring` - Marks an entity as needing to find a target and contains data about the search.
  - `ShootAttackAuthoring` - Marks an entity as being able to shoot and contains data about shooting.
- `Scripts\Systems` - Contains scripts that define systems used in ECS.
  - `ResetEventsSystem` - Resets events at the end of the frame.
  - `UnitMoverSystem` - Handles movable entities towards their destination.
  - `SelectedVisualSystem` - Updates a selectable unit's "selected" visual based on whether it is selected or not.
  - `FindTargetSystem` - Finds targets for entities that need to find a target.
  - `ShootAttackSystem` - Handles shooting for entities that can shoot.
    - Uses the `EntityReferences` to instantiate bullets.



# Changes
- The project started from the "VR" template.
- `AutoHand` package was imported.
- `Polygon sci-fi city` package was imported.
- Created the `Playpen` scene as a copy of the `XRHands` from the AutoHand OpenXR package.
- The `Auto Hand Player Container/TrackerOffsets/Highlight Projection (R)/OuterMesh` uses the `Playpen\Assets\AutoHand\Examples\Materials\Highlight\Hands_transparent.mat` material, which uses the `Playpen\Assets\[Playpen]\Shaders\Hands_transparfent.shader` shader. This shader was not compiling, and has been replaced with the `Playpen\Assets\[Playpen]\Shaders\Hands_transparent.shader` shader.
- Imported the `DOTS_RTS_Course_VisualAssets_Part1.unitypackage` package.
- Created `Assets/[Playpen]/DOTS/DOTS URP Config` from `Assets/Settings/Project Configuration/Quality URP Config`.
- As suggested on the Unity "Entities project setup" page, "Enter Play Mode Settings" in "Project Settings/Editor" was set to "Do not reload Domain or Scene".
- "Active Input Handling" in "Project Settings/Player" was set to "Both".

# Notes

## Graphics
- A "DOTS" quality level has been created and is the default quality level for PC.
- The "DOTS" quality level uses the "DOTS URP Config" render pipeline asset.
- The "DOTS URP Config" render pipeline asset uses the "DOTS URP Preset" Universal Render Data.

## DOTS
- `ISystem` provides an interface for unmanaged systems.
- `ISystemBase` provides an interface for managed systems.
- `SystemAPI.GetComponentRW` accesses memory outside of the containing query, which somewhat breaks DOTS' ECS paradigm.
- Companion GameObjects are used to handle functionality that is not yet available in DOTS, such as lights.
- `RenderMeshArray` is a shared component that allows multiple entities to share the same mesh and material data.

# Issues
- Shadow not showing
  - Move the camera close. If you can see the shadow, then the shadow distance is too low. This is set in the active render pipeline asset.

# Units
Units are created from a prefab. The root GameObject of this prefab has a `UnitMover` and a `Selected` component.