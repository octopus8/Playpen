using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using BoxCollider = UnityEngine.BoxCollider;
using Material = UnityEngine.Material;

namespace RTS
{
    public class BuildingPlacement : MonoBehaviour
    {
        public static BuildingPlacement Instance { get; private set; }
        public event EventHandler OnActiveBuildingChanged;
        
        [SerializeField]
        private BuildingScriptableObject buildingScriptableObject;

        [SerializeField] private Material ghostMaterial;
        
        private Transform ghostTransform;
        
        
        
        private void Awake()
        {
            Instance = this;
        }
    
        private void Update()
        {
            if (ghostTransform != null)
            {
                Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
                ghostTransform.position = mousePosition;
            }
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            if (buildingScriptableObject.IsNone())
            {
                return;
            }

            if (Input.GetMouseButton(1))
            {
                SetActiveBuilding(RTSGame.Instance.buildings.none);
            }
            
            
            if (Input.GetMouseButtonDown(0))
            {
                if (CanPlaceBuilding())
                {
                    Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();


                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                    EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntityPrefabSet));
                    EntityPrefabSet entityPrefabSet = entityQuery.GetSingleton<EntityPrefabSet>();
                    Entity entity = entityManager.Instantiate(buildingScriptableObject.GetBuilding(entityPrefabSet));
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
            
            distanceHitList.Clear();
            if (collisionWorld.OverlapSphere(mousePosition, buildingScriptableObject.buildingDistanceMin,
                    ref distanceHitList, collisionFilter))
            {
                foreach (DistanceHit distanceHit in distanceHitList)
                {
                    if (entityManager.HasComponent<BuildingType>(distanceHit.Entity))
                    {
                        BuildingType buildingType = entityManager.GetComponentData<BuildingType>(distanceHit.Entity);
                        if (buildingType.buildingType == buildingScriptableObject.buildingType)
                        {
                            return false;
                        }
                    }
                }
            }
                
            
            return true;
        }
        
        public BuildingScriptableObject GetActiveBuilding()
        {
            return buildingScriptableObject;
        }
        
        public void SetActiveBuilding(BuildingScriptableObject building)
        {
            buildingScriptableObject = building;

            if (ghostTransform != null)
            {
                Destroy(ghostTransform.gameObject);
            }
            
            if (!buildingScriptableObject.IsNone())
            {
                ghostTransform = Instantiate(buildingScriptableObject.ghostPrefab);
                foreach (Renderer renderer in ghostTransform.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = ghostMaterial;
                }
            }
            
            
            OnActiveBuildingChanged?.Invoke(this, EventArgs.Empty);
        }


    }
    
}
