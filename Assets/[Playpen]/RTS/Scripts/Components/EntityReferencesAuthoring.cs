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

        
        class Baker : Baker<EntityReferencesAuthoring>
        {
            public override void Bake(EntityReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityReferences
                {
                    bulletEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    zombieEntity = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic),
                    shootLightEntity = GetEntity(authoring.shootLightPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct EntityReferences : IComponentData
    {
        public Entity bulletEntity;
        public Entity zombieEntity;
        public Entity shootLightEntity;
    }
}


