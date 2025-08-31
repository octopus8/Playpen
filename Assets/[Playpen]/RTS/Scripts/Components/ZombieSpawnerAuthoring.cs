using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        public float spawnInterval = 5f;

        class Baker : Baker<ZombieSpawnerAuthoring>
        {
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ZombieSpawner
                {
                    spawnInterval = authoring.spawnInterval
                });
            }
        }
    }

    public struct ZombieSpawner : IComponentData
    {
        public float timer;
        public float spawnInterval;
    }

}


