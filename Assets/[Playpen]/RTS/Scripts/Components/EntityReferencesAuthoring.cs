using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for entity references.
    /// Only a single instance of this ECS component should exist in the scene.
    /// </summary>
    public class EntityReferencesAuthoring : MonoBehaviour
    {
        /// <summary>The bullet prefab.</summary>
        [Tooltip("The bullet prefab.")]
        [SerializeField] private GameObject bulletPrefab;
        
        /// <summary>The zombie prefab.</summary>
        [Tooltip("The zombie prefab.")]
        [SerializeField] private GameObject zombiePrefab;
        
        /// <summary>The shoot light prefab.</summary>
        [Tooltip("The shoot light prefab.")]
        [SerializeField] private GameObject shootLightPrefab;

        /// <summary>The soldier prefab.</summary>
        [Tooltip("The soldier prefab.")]
        [SerializeField] private GameObject soldierPrefab;
        
        /// <summary>The scout prefab.</summary>
        [Tooltip("The scout prefab.")]
        [SerializeField] private GameObject scoutPrefab;

        /// <summary>The building tower prefab.</summary>
        [Tooltip("The building tower prefab.")]
        [SerializeField] private GameObject buildingTowerPrefab;
        
        /// <summary>The building barracks prefab.</summary>
        [Tooltip("The building barracks prefab.")]
        [SerializeField] private GameObject buildingBarracksPrefab;
        
        class Baker : Baker<EntityReferencesAuthoring>
        {
            public override void Bake(EntityReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityReferences
                {
                    bulletEntityPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    zombieEntityPrefab = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic),
                    shootLightEntityPrefab = GetEntity(authoring.shootLightPrefab, TransformUsageFlags.Dynamic),
                    soldierEntityPrefab = GetEntity(authoring.soldierPrefab, TransformUsageFlags.Dynamic),
                    scoutEntityPrefab = GetEntity(authoring.scoutPrefab, TransformUsageFlags.Dynamic),
                    buildingTowerPrefab = GetEntity(authoring.buildingTowerPrefab, TransformUsageFlags.Dynamic),
                    buildingBarracksPrefab = GetEntity(authoring.buildingBarracksPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct EntityReferences : IComponentData
    {
        /// <summary>The bullet prefab entity.</summary>
        public Entity bulletEntityPrefab;
        
        /// <summary>The zombie prefab entity.</summary>
        public Entity zombieEntityPrefab;
        
        /// <summary>The shoot light prefab entity.</summary>
        public Entity shootLightEntityPrefab;

        /// <summary>The soldier prefab entity.</summary>
        public Entity soldierEntityPrefab;
        
        /// <summary>The scout prefab entity.</summary>
        public Entity scoutEntityPrefab;

        public Entity buildingTowerPrefab;
        
        public Entity buildingBarracksPrefab;

    }
}


