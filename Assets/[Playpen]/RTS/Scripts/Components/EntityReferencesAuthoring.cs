using Unity.Entities;
using UnityEngine;


namespace RTS
{

    public class EntityReferencesAuthoring : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public GameObject zombiePrefab;

        class Baker : Baker<EntityReferencesAuthoring>
        {
            public override void Bake(EntityReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityReferences
                {
                    bulletPrefabEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    zombiePrefabEntity = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct EntityReferences : IComponentData
    {
        public Entity bulletPrefabEntity;
        public Entity zombiePrefabEntity;
    }
}


