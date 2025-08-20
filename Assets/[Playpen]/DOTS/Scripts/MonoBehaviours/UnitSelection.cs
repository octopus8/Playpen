using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public static UnitSelection Instance { get; private set; }
    
    public EventHandler OnSelectionAreaStart;
    public EventHandler OnSelectionAreaEnd;
    private Vector2 startMousePosition;
    
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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }
        
        
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMoverDOTS, SelectedDOTS>()
                .Build(entityManager);
            NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<UnitMoverDOTS> unitMoverDataArray = entityQuery.ToComponentDataArray<UnitMoverDOTS>(Allocator.Temp);
            for (int i = 0; i < unitMoverDataArray.Length; i++)
            {
                UnitMoverDOTS unitMover = unitMoverDataArray[i];
                unitMover.targetPosition = mousePosition;
                unitMoverDataArray[i] = unitMover;
            }
            entityQuery.CopyFromComponentDataArray(unitMoverDataArray);
        }
    }

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
