using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using BoxCollider = UnityEngine.BoxCollider;

namespace RTS
{
    public class BuildingPlacement : MonoBehaviour
    {
        
        [SerializeField]
        private BuildingScriptableObject buildingScriptableObject;
    
        private void Update()
        {
            if (CanPlaceBuilding())
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();


                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                    EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntityReferences));
                    EntityReferences entityReferences = entityQuery.GetSingleton<EntityReferences>();
                    Entity entity = entityManager.Instantiate(entityReferences.buildingTowerPrefab);
                    entityManager.SetComponentData(entity, LocalTransform.FromPosition(mousePosition));
                }
            }
        }


        private bool CanPlaceBuilding()
        {
            Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            CollisionFilter collisionFilter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << RTSGame.BUILDINGS_LAYER,
                GroupIndex = 0
            };

            BoxCollider boxCollider = buildingScriptableObject.prefab.GetComponent<BoxCollider>();
            float bonusExtents = 1.1f;
            NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
            if (collisionWorld.OverlapBox(mousePosition, Quaternion.identity, boxCollider.size * 0.5f * bonusExtents,
                    ref distanceHitList, collisionFilter))
            {
                return false;
            }
            
            return true;
        }


    }
    
}
