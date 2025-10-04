using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Singleton Authoring component for entity references.
    /// </summary>
    public class EntityPrefabSetAuthoring : MonoBehaviour
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
        
        class Baker : Baker<EntityPrefabSetAuthoring>
        {
            public override void Bake(EntityPrefabSetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityPrefabSet
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

    public struct EntityPrefabSet : IComponentData
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


