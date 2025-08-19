using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMoverDOTS>()
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
}
