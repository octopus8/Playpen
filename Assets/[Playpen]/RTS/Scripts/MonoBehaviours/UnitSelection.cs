using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;


namespace RTS
{
    /// <summary>
    /// Singleton that handles unit selection and setting target positions for selected units.
    /// Provides access to the selection area rectangle, used by the UI to visually represent the selection area.
    /// Unit selection is done through mouse input, with left-click for selection and right-click for setting target positions.
    /// Supports both single selection (clicking on a unit) and multiple selection (clicking and dragging to create a selection area).
    /// When doing a multiple selection, unit positions are converted to screen space to determine if they are within the selection area.
    /// When doing a single selection, a raycast is performed to determine if a unit was clicked on. Additionally, right-clicking on an enemy unit will set it as the target for all selected units.
    /// This component exposes events for selection area start and end, allowing other components to respond to these events (e.g., updating the UI).
    /// </summary>
    public class UnitSelection : MonoBehaviour
    {
        #region Static Class Variables
        
        /// <summary> Singleton instance of UnitSelection. </summary>
        public static UnitSelection Instance { get; private set; }
        
        #endregion

        
        #region Public Event Handlers

        /// <summary> Event handler for selection area start. </summary>
        public EventHandler OnSelectionAreaStart;

        /// <summary> Event handler for selection area end. </summary>
        public EventHandler OnSelectionAreaEnd;

        /// <summary> Event handler for when the selected units change. </summary>
        public EventHandler OnSelectedChanged;

        #endregion

        
        #region Private Class Variables

        /// <summary> Stores the starting mouse position for selection area. </summary>
        private Vector2 startMousePosition;

        #endregion


        #region MonoBehaviour Callbacks

        /// <summary>
        /// Stores the singleton instance of UnitSelection.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        /// <summary>
        /// Handle selection area start and end events, and set target positions for selected units.
        /// This method is called every frame to check for mouse input and update the selection area.
        /// </summary>
        void Update()
        {
            // Ignore input if the mouse is over a UI element.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            // Ignore input if a building is being placed.
            if (!BuildingPlacement.Instance.GetActiveBuilding().IsNone())
            {
                return;
            }
            
            // Handle selection area start.
            if (Input.GetMouseButtonDown(0))
            {
                startMousePosition = Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }
            
            // Handle selection area end.
            if (Input.GetMouseButtonUp(0))
            {
                HandleSelectionEnd();
            }

            // Handle setting target positions for selected units.
            if (Input.GetMouseButtonDown(1))
            {
                HandleSetTarget();
            }
        }
        
        #endregion


        #region Public Methods

        /// <summary>
        /// Gets the rectangle area of the selection based on the start mouse position and current mouse position.
        /// This method calculates the lower-left and upper-right corners of the selection rectangle
        /// and returns a Rect object representing the selection area.
        /// This is used by the UI to visually represent the selection area on the screen.
        /// </summary>
        public Rect GetSelectionAreaRect()
        {
            Vector2 lowerLeft = new Vector2(
                Mathf.Min(startMousePosition.x, Input.mousePosition.x),
                Mathf.Min(startMousePosition.y, Input.mousePosition.y));
            Vector2 upperRight = new Vector2(
                Mathf.Max(startMousePosition.x, Input.mousePosition.x),
                Mathf.Max(startMousePosition.y, Input.mousePosition.y));
            return new Rect(lowerLeft.x, lowerLeft.y, upperRight.x - lowerLeft.x, upperRight.y - lowerLeft.y);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Generates an array of move positions around a target position for a given number of units.
        /// The positions are arranged in concentric rings around the target position to avoid overlap.
        /// </summary>
        private NativeArray<float3> GenerateMovePositionsArray(float3 targetPosition, int positionCount)
        {
            // If no positions are needed, return the empty array.
            NativeArray<float3> movePositions = new NativeArray<float3>(positionCount, Allocator.Temp);
            if (positionCount == 0)
            {
                return movePositions;
            }
            
            // If only one position is needed, return the array with the target position.
            movePositions[0] = targetPosition;
            if (positionCount == 1)
            {
                return movePositions;
            }

            // Generate positions in concentric rings around the target position.
            float ringSize = 2.2f;
            int currentRing = 0;
            int currentPositionIndex = 1;
            while (currentPositionIndex < positionCount)
            {
                // Generate positions in the current ring.
                int positionsInRing = 3 + currentRing * 2;
                for (int i = 0; i < positionsInRing; i++)
                {
                    // Calculate the position in the ring using polar coordinates.
                    float angle = i * (math.PI2 / positionsInRing);
                    float3 vector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (currentRing + 1), 0, 0));
                    float3 ringPosition = targetPosition + vector;
                    
                    // Add the position to the array.
                    movePositions[currentPositionIndex] = ringPosition;
                    
                    // If we have generated enough positions, break out of the loop.
                    currentPositionIndex++;
                    if (currentPositionIndex >= positionCount)
                    {
                        break;
                    }
                }

                // Move to the next ring.
                currentRing++;
            }

            // Return the array of move positions.
            return movePositions;
        }

        
        /// <summary>
        /// Sets the target for all selected units to the mouse position or to the clicked enemy unit.
        /// </summary>
        private void HandleSetTarget()
        {
            // Get the mouse position.
            Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
            
            // Check if the click was on an enemy unit using a raycast at the mouse position.
            // If the click was on an enemy unit, set it as the target for all selected units.
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput()
            {
                Start = ray.GetPoint(0f),
                End = ray.GetPoint(10000f),
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1 << RTSGame.UNITS_LAYER | 1u << RTSGame.BUILDINGS_LAYER,
                    GroupIndex = 0
                }
            };
            
            // If the raycast hits an entity with a Faction component, check if it's an enemy unit, and set it as the target.
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
            {
                // If the hit entity has a Faction component, check if it's an enemy unit.
                if (entityManager.HasComponent<Faction>(hit.Entity))
                {
                    // The click was on an enemy unit; set it as the target for all selected units.
                    Faction faction = entityManager.GetComponentData<Faction>(hit.Entity);
                    if (faction.factionType == FactionType.Enemy)
                    {
                        HandleTargetSingleUnit(entityManager, entityQuery, hit.Entity);
                        return;
                    }
                }
            }
            
            // Query all selected units with a UnitMoverOverride and TargetOverride component.
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitSelected>()
                .WithPresent<UnitMoverOverride, TargetOverride>()
                .Build(entityManager);
            
            // Convert the query results to NativeArrays of entities.
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            // Create an array of UnitMoverOverride components to store the target positions.
            NativeArray<UnitMoverOverride> moveOverrideArray = entityQuery.ToComponentDataArray<UnitMoverOverride>(Allocator.Temp);
            
            // Create an array of TargetOverride components to clear any existing target entities.
            NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
            
            // Generate move position array around the mouse position.
            NativeArray<float3> movePositions = GenerateMovePositionsArray(mousePosition, moveOverrideArray.Length);
            
            // Iterate through the move override array and set the target position for each selected unit.
            for (int i = 0; i < moveOverrideArray.Length; i++)
            { 
                // Get the UnitMoverOverride component.
                UnitMoverOverride unitMoverOverride = moveOverrideArray[i];
                
                // Set the target position to the corresponding move position.
                unitMoverOverride.overrideDestination = movePositions[i];
                
                // Set the modified UnitMoverOverride back to the array.
                moveOverrideArray[i] = unitMoverOverride;

                // Enable the UnitMoverOverride component to activate the override.
                entityManager.SetComponentEnabled<UnitMoverOverride>(entityArray[i], true);
                
                // Get the TargetOverride component.
                TargetOverride targetOverride = targetOverrideArray[i];
                
                // Set the target entity to null to clear any existing target.
                targetOverride.targetEntity = Entity.Null;
                
                // Set the modified TargetOverride back to the array.
                targetOverrideArray[i] = targetOverride;
            }

            // Copy the modified data back to the entity query.
            entityQuery.CopyFromComponentDataArray(moveOverrideArray);
            entityQuery.CopyFromComponentDataArray(targetOverrideArray);
            
            // Create an entity query for selected barracks to set the rally position offset.
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitSelected, BuildingBarracks, LocalTransform>()
                .Build(entityManager);
            
            // Create an array of BuildingBarracks components to set the rally position offset.
            NativeArray<BuildingBarracks> unitSpawners = entityQuery.ToComponentDataArray<BuildingBarracks>(Allocator.Temp);
            
            // Create an array of LocalTransform components to get the positions of the unit spawners.
            NativeArray<LocalTransform> localTransforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            
            // Iterate through the array of UnitSpawners and set the rally position offset for each selected unit spawner.
            for (int i = 0; i < unitSpawners.Length; i++)
            {
                BuildingBarracks buildingBarracks = unitSpawners[i];
                buildingBarracks.rallyPositionOffset = (float3)mousePosition - localTransforms[i].Position;
                unitSpawners[i] = buildingBarracks;
            }

            // Copy the modified data back to the entity query.
            entityQuery.CopyFromComponentDataArray(unitSpawners);
        }


        /// <summary>
        /// Sets the target entity for all selected units to a single enemy unit.
        /// Disables the UnitMoverOverride component to allow normal movement towards the target.
        /// </summary>
        private void HandleTargetSingleUnit(EntityManager entityManager, EntityQuery entityQuery, Entity targetEntity)
        {
            // Query all selected units with a TargetOverride component.
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitSelected>()
                .WithPresent<TargetOverride>()
                .Build(entityManager);

            // Convert the query results to a NativeArray of UnitMover components.
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            
            // Create an array of TargetOverride components to set the target entity.
            NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
            
            // Iterate through the TargetOverride array and set the target entity for each selected unit.
            for (int i = 0; i < targetOverrideArray.Length; i++)
            {
                // Get the TargetOverride component.
                TargetOverride targetOverride = targetOverrideArray[i];
                
                // Set the target entity to the clicked enemy unit.
                targetOverride.targetEntity = targetEntity;
                
                // Set the modified TargetOverride back to the array.
                targetOverrideArray[i] = targetOverride;
                
                // Disable the UnitMoverOverride component to allow normal movement towards the target.
                entityManager.SetComponentEnabled<UnitMoverOverride>(entityArray[i], false);
            }

            // Copy the modified data back to the entity query.
            entityQuery.CopyFromComponentDataArray(targetOverrideArray);
        }
        

        /// <summary>
        /// Handles the end of a selection area by updating the selection state of units.
        /// Deselects all currently selected units, then selects units within the selection area for multiple selection,
        /// or selects a single unit if the selection area is small enough.
        /// Triggers events for selection area end and selected changed.
        /// </summary>
        private void HandleSelectionEnd()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // Create an entity query for all entities with the UnitSelected component.
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitSelected>()
                .Build(entityManager);
            
            // Create an array of entities to modify the selection state.
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            
            // Create an array of UnitSelected components to modify the selection state.
            NativeArray<UnitSelected> selectedDataArray = entityQuery.ToComponentDataArray<UnitSelected>(Allocator.Temp);
            
            // Iterate through the array and set all units as not selected.
            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<UnitSelected>(entityArray[i], false);
                
                // Get the UnitSelected component.
                UnitSelected unitSelected = selectedDataArray[i];
                
                // Set the unit as not selected.
                unitSelected.onDeselected = true;
                
                // Update the UnitSelected component in the array.
                selectedDataArray[i] = unitSelected;
                
                // Set the modified UnitSelected back to the entity.
                entityManager.SetComponentData(entityArray[i], unitSelected);
            }

            // Determine if the selection area is large enough for multiple selection.
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaRelativeSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSizeMin = 40f;
            bool isMultipleSelection = selectionAreaRelativeSize > multipleSelectionSizeMin;

            // If it's a multiple selection, handle multiple selection logic.
            if (isMultipleSelection)
            {
                HandleMultipleSelection(entityManager, entityQuery, entityArray, selectionAreaRect);
            }
            
            // Otherwise, handle single selection logic.
            else
            {
                HandleSingleSelection(entityManager, entityQuery);
            }

            // Call the callback for selection area end.
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            
            // Call the callback for selected changed.
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        
        /// <summary>
        /// Handles multiple unit selection by checking which units are within the selection area rectangle.
        /// Sets the UnitSelected component for units within the selection area to mark them as selected.
        /// </summary>
        private void HandleMultipleSelection(EntityManager entityManager, EntityQuery entityQuery, NativeArray<Entity> entityArray, Rect selectionAreaRect)
        {
            // Create an entity query for all units with a LocalTransform component and a Unit component.
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, Unit>()
                .WithPresent<UnitSelected>()
                .Build(entityManager);
            
            // Create arrays of entities and LocalTransform components to check their positions.
            entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            
            // Create an array of LocalTransform components to get the positions of the units.
            NativeArray<LocalTransform> unitLocalTransformDataArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                    
            // Iterate through query results and set units within the selection area as selected.
            for (int i = 0; i < unitLocalTransformDataArray.Length; i++)
            {
                // Get the unit screen position.
                LocalTransform localTransform = unitLocalTransformDataArray[i];
                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
                        
                // If the unit screen position is within the selection area, set it as selected.
                if (selectionAreaRect.Contains(unitScreenPosition))
                {
                    // Enable the UnitSelected component to mark the unit as selected.
                    entityManager.SetComponentEnabled<UnitSelected>(entityArray[i], true);
                    
                    // Get the UnitSelected component.
                    UnitSelected unitSelected = entityManager.GetComponentData<UnitSelected>(entityArray[i]);

                    // Set the unit as selected.
                    unitSelected.onSelected = true;
                    
                    // Set the modified UnitSelected back to the entity.
                    entityManager.SetComponentData(entityArray[i], unitSelected);
                }
            }
        }

        
        /// <summary>
        /// Handles single unit selection by performing a raycast at the mouse position to check if a unit was clicked.
        /// If a unit with a UnitSelected component is hit, it is marked as selected.
        /// </summary>
        private void HandleSingleSelection(EntityManager entityManager, EntityQuery entityQuery)
        {
            // Create an entity query for all units with a UnitSelected component.
            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            
            // Get the PhysicsWorldSingleton to perform raycasts.
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

            // Perform a raycast at the mouse position to check if a unit was clicked.
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput()
            {
                Start = ray.GetPoint(0f),
                End = ray.GetPoint(10000f),
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1 << RTSGame.UNITS_LAYER | 1u << RTSGame.BUILDINGS_LAYER,
                    GroupIndex = 0
                }
            };
            
            // If the raycast hits an entity with a UnitSelected component, set it as selected.
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
            {
                // If the hit entity has a UnitSelected component, set it as selected.
                if (entityManager.HasComponent<UnitSelected>(hit.Entity))
                {
                    // Enable the UnitSelected component to mark the unit as selected.
                    entityManager.SetComponentEnabled<UnitSelected>(hit.Entity, true);
                    
                    // Get the UnitSelected component.
                    UnitSelected unitSelected = entityManager.GetComponentData<UnitSelected>(hit.Entity);
                    
                    // Set the unit as selected.
                    unitSelected.onSelected = true;
                    
                    // Set the modified UnitSelected back to the entity.
                    entityManager.SetComponentData(hit.Entity, unitSelected);
                }
            }
        }


        #endregion
    }
}
