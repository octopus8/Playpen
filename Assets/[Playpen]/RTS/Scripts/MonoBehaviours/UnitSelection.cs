using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;



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

        private NativeArray<float3> GenerateMovePositionsArray(float3 targetPosition, int positionCount)
        {
            NativeArray<float3> movePositions = new NativeArray<float3>(positionCount, Allocator.Temp);
            if (positionCount == 0)
            {
                return movePositions;
            }

            movePositions[0] = targetPosition;
            if (positionCount == 1)
            {
                return movePositions;
            }

            float ringSize = 2.2f;
            int currentRing = 0;
            int currentPositionIndex = 1;
            while (currentPositionIndex < positionCount)
            {
                int positionsInRing = 3 + currentRing * 2;

                for (int i = 0; i < positionsInRing; i++)
                {
                    float angle = i * (math.PI2 / positionsInRing);
                    float3 vector = math.rotate(quaternion.RotateY(angle),
                        new float3(ringSize * (currentRing + 1), 0, 0));
                    float3 ringPosition = targetPosition + vector;

                    movePositions[currentPositionIndex] = ringPosition;
                    currentPositionIndex++;

                    if (currentPositionIndex >= positionCount)
                    {
                        break;
                    }
                }

                currentRing++;
            }

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
                    CollidesWith = 1 << RTSGame.UNITS_LAYER,
                    GroupIndex = 0
                }
            };
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
            {
                if (entityManager.HasComponent<Unit>(hit.Entity))
                {
                    // The click was on an enemy unit; set it as the target for all selected units.
                    Unit unit = entityManager.GetComponentData<Unit>(hit.Entity);
                    if (unit.faction == Faction.Zombie)
                    {
                        HandleTargetSingleUnit(entityManager, entityQuery, hit.Entity);
                        return;
                    }
                }
            }

            // An enemy unit was not clicked; set the target position for all selected units.
            // Iterate through an array of MoveOverride components and set the target position for each selected unit.
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selected>()
                .WithPresent<MoveOverride, TargetOverride>()
                .Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);
            NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
            NativeArray<float3> movePositions = GenerateMovePositionsArray(mousePosition, moveOverrideArray.Length);
            for (int i = 0; i < moveOverrideArray.Length; i++)
            {
                MoveOverride moveOverride = moveOverrideArray[i];
                moveOverride.targetPosition = movePositions[i];
                moveOverrideArray[i] = moveOverride;
                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);
                
                TargetOverride targetOverride = targetOverrideArray[i];
                targetOverride.targetEntity = Entity.Null;
                targetOverrideArray[i] = targetOverride;
            }

            // Copy the modified data back to the entity query.
            entityQuery.CopyFromComponentDataArray(moveOverrideArray);
            entityQuery.CopyFromComponentDataArray(targetOverrideArray);
        }


        private void HandleTargetSingleUnit(EntityManager entityManager, EntityQuery entityQuery, Entity targetEntity)
        {
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selected>()
                .WithPresent<TargetOverride>()
                .Build(entityManager);

            // Convert the query results to a NativeArray of UnitMover components.
            // Iterate through the array and set the target position for each selected unit.
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<TargetOverride> targetOverrideArray =
                entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
            for (int i = 0; i < targetOverrideArray.Length; i++)
            {
                TargetOverride targetOverride = targetOverrideArray[i];
                targetOverride.targetEntity = targetEntity;
                targetOverrideArray[i] = targetOverride;
                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], false);
            }

            // Copy the modified data back to the entity query.
            entityQuery.CopyFromComponentDataArray(targetOverrideArray);
        }
        
        
        private void HandleSelectionEnd()
        {
            // Set all ECS unit entities as unselected before processing the selection area.
            // This ensures that only the units within the selection area are selected.
            // It also prevents previously selected units from remaining selected after the selection area is cleared.
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selected>()
                .Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedDataArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                Selected selected = selectedDataArray[i];
                selected.onDeselected = true;
                selectedDataArray[i] = selected;
                entityManager.SetComponentData(entityArray[i], selected);
            }

            // Determine if the selection area is a multiple selection or a single selection.
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaRelativeSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSizeMin = 40f;
            bool isMultipleSelection = selectionAreaRelativeSize > multipleSelectionSizeMin;

            // Handle multiple selection.
            if (isMultipleSelection)
            {
                HandleMultipleSelection(entityManager, entityQuery, entityArray, selectionAreaRect);
            }
            
            // Single select
            else
            {
                HandleSingleSelection(entityManager, entityQuery);
            }

            // Call the callback for selection area end.
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        
        private void HandleMultipleSelection(EntityManager entityManager, EntityQuery entityQuery, NativeArray<Entity> entityArray, Rect selectionAreaRect)
        {
            // Set ECS unit entities within the selection area as selected.
            // This is done by checking if the unit's screen position is within the selection area rectangle.
            // If it is, we enable the Selected component for that unit.
            // This allows the UI to visually represent the selected units.
            entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, Unit>()
                .WithPresent<Selected>()
                .Build(entityManager);
            entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<LocalTransform> unitLocalTransformDataArray =
                entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                    
            // Iterate through query results and set units within the selection area as selected.
            for (int i = 0; i < unitLocalTransformDataArray.Length; i++)
            {
                // Get the unit screen position.
                LocalTransform localTransform = unitLocalTransformDataArray[i];
                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
                        
                // If the unit screen position is within the selection area, set it as selected.
                if (selectionAreaRect.Contains(unitScreenPosition))
                {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                    Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                    selected.onSelected = true;
                    entityManager.SetComponentData(entityArray[i], selected);
                }
            }
        }

        
        private void HandleSingleSelection(EntityManager entityManager, EntityQuery entityQuery)
        {
            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
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
                    CollidesWith = 1 << RTSGame.UNITS_LAYER,
                    GroupIndex = 0
                }
            };
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
            {
                // If the hit entity is a unit, set it as selected.
                if (entityManager.HasComponent<Unit>(hit.Entity) &&
                    entityManager.HasComponent<Selected>(hit.Entity))
                {
                    entityManager.SetComponentEnabled<Selected>(hit.Entity, true);
                    Selected selected = entityManager.GetComponentData<Selected>(hit.Entity);
                    selected.onSelected = true;
                    entityManager.SetComponentData(hit.Entity, selected);
                }
            }
        }


        #endregion
    }
}
