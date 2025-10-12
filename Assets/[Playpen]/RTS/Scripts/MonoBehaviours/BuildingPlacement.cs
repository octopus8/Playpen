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
    /// <summary>
    /// Singleton class that manages building placement in the RTS game.
    /// Handles selecting buildings to place, showing a ghost preview, and placing buildings in the world.
    /// </summary>
    public class BuildingPlacement : MonoBehaviour
    {
        /// <summary> The currently selected building scriptable object. </summary>
        [Tooltip("The currently selected building scriptable object.")]
        [SerializeField] private BuildingScriptableObject buildingScriptableObject;

        /// <summary> Material used for the ghost building preview. </summary>
        [Tooltip("Material used for the ghost building preview.")]
        [SerializeField] private Material ghostMaterial;
        
        /// <summary> Singleton instance of the BuildingPlacement class. </summary>
        public static BuildingPlacement Instance { get; private set; }
        
        /// <summary> Event triggered when the active building changes. </summary>
        public event EventHandler OnActiveBuildingChanged;
        
        /// <summary> Transform of the ghost building preview. </summary>
        private Transform ghostTransform;
        
        
        /// <summary>
        /// Initializes the singleton instance.
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }
    
        /// <summary>
        /// Updates the ghost building position to follow the mouse cursor.
        /// Handles input for placing buildings and cancelling placement.
        /// </summary>
        private void Update()
        {
            // If there is an active building, update the ghost position to follow the mouse.
            if (ghostTransform != null)
            {
                Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
                ghostTransform.position = mousePosition;
            }
            
            // If the mouse is over a UI element, do not process building placement input.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            // If there is no active building, exit early.
            if (buildingScriptableObject.IsNone())
            {
                return;
            }

            // If the right mouse button is clicked, cancel building placement.
            if (Input.GetMouseButton(1))
            {
                SetActiveBuilding(RTSGame.Instance.buildings.none);
            }
            
            // If the left mouse button is clicked, attempt to place the building.
            if (Input.GetMouseButtonDown(0))
            {
                // If the building can be placed at the current mouse position, instantiate it.
                if (CanPlaceBuilding())
                {
                    // Get the EntityPrefabSet singleton to access building prefabs.
                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntityPrefabSet));
                    EntityPrefabSet entityPrefabSet = entityQuery.GetSingleton<EntityPrefabSet>();

                    // Instantiate the building entity at the mouse position.
                    Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
                    Entity entity = entityManager.Instantiate(buildingScriptableObject.GetBuilding(entityPrefabSet));
                    entityManager.SetComponentData(entity, LocalTransform.FromPosition(mousePosition));
                }
            } 
        }


        /// <summary>
        /// Determines if the building can be placed at the current mouse position.
        /// Checks for collisions with other buildings and ensures minimum distance requirements are met.
        /// </summary>
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
        
        
        /// <summary>
        /// Gets the currently active building scriptable object.
        /// </summary>
        public BuildingScriptableObject GetActiveBuilding()
        {
            return buildingScriptableObject;
        }
        
        
        /// <summary>
        /// Sets the active building to the given building scriptable object.
        /// Updates the ghost preview and triggers the active building changed event.
        /// </summary>
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
