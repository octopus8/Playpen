# RTS

## Overview
The `RTS` project is a simple real-time strategy (RTS) game implemented using Unity's Entity Component System (ECS). The project demonstrates how to set up a basic RTS game with unit selection, movement, and attacking mechanics using ECS principles.

## Scripts
- All scripts for this project are in the `Playpen\Assets\[Playpen]\Scripts` folder.
- At the root, the `Scripts` directory contains scripts that contain general app data structures.
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
## My Understanding of ECS
- ECS stands for Entity Component System.
- An Entity is a general purpose object. It is just an ID.
- A Component is a data container that holds data. It does not contain any logic.
- A System contains logic that processes entities with specific components.
- In Unity's ECS, components are structs that implement `IComponentData`.
- Systems can be implemented as `ISystem` (unmanaged) or `SystemBase` (managed).
- Systems use queries to find entities with specific components.


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