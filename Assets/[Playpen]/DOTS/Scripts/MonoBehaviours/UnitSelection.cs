using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


/// <summary>
/// Singleton that handles unit selection and setting target positions for selected units.
/// Provides access to the selection area rectangle, used by the UI to visually represent the selection area.
/// </summary>
public class UnitSelection : MonoBehaviour
{
    /// <summary> Singleton instance of UnitSelection. </summary>
    public static UnitSelection Instance { get; private set; }

    /// <summary> Event handler for selection area start. </summary>
    public EventHandler OnSelectionAreaStart;
    
    /// <summary> Event handler for selection area end. </summary>
    public EventHandler OnSelectionAreaEnd;
    
    /// <summary> Stores the starting mouse position for selection area. </summary>
    private Vector2 startMousePosition;

    
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
        // Handle selection area start and end events.
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }
        
        
        if (Input.GetMouseButtonUp(0))
        {
            // Set all units as unselected before processing the selection area.
            // This ensures that only the units within the selection area are selected.
            // It also prevents previously selected units from remaining selected after the selection area is cleared.
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selected>()
                .Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            for (int i=0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            }
            
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaRelativeSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSizeMin = 40f;
            bool isMultipleSelection = selectionAreaRelativeSize > multipleSelectionSizeMin;

            if (isMultipleSelection)
            {
                // Set units within the selection area as selected.
                // This is done by checking if the unit's screen position is within the selection area rectangle.
                // If it is, we enable the Selected component for that unit.
                // This allows the UI to visually represent the selected units.
                entityQuery = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<LocalTransform, Unit>()
                    .WithPresent<Selected>() 
                    .Build(entityManager);
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> unitLocalTransformDataArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                for (int i = 0; i < unitLocalTransformDataArray.Length; i++)
                {
                    LocalTransform localTransform = unitLocalTransformDataArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                    }
                }
            }
            // Single select
            else
            {
                const int unitLayer = 10; // Layer for units, set in Unity Editor.
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
                        CollidesWith = 1 << unitLayer,
                        GroupIndex = 0
                    }
                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
                {
                    // If the hit entity is a unit, set it as selected.
                    if (entityManager.HasComponent<Unit>(hit.Entity))
                    {
                        entityManager.SetComponentEnabled<Selected>(hit.Entity, true);
                    }
                }
            }
            
            
            // Call the callback for selection area end.
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }
        
        // Handle setting target positions for selected units.
        if (Input.GetMouseButtonDown(1))
        {
            // Get all the entities with UnitMover and Selected components.
            Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover, Selected>()
                .Build(entityManager);
            
            // Convert the query results to a NativeArray of UnitMover components.
            // Iterate through the array and set the target position for each selected unit.
            NativeArray<UnitMover> unitMoverDataArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            for (int i = 0; i < unitMoverDataArray.Length; i++)
            {
                UnitMover unitMover = unitMoverDataArray[i];
                unitMover.targetPosition = mousePosition;
                unitMoverDataArray[i] = unitMover;
            }
            
            // Copy the modified data back to the entity query.
            entityQuery.CopyFromComponentDataArray(unitMoverDataArray);
        }
    }

    
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
}
