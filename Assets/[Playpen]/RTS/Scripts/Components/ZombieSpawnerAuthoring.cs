using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        public float spawnInterval = 5f;
        public float randomWalkingDistanceMin;
        public float randomWalkingDistanceMax;

        class Baker : Baker<ZombieSpawnerAuthoring>
        {
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ZombieSpawner
                {
                    spawnInterval = authoring.spawnInterval,
                    randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                    randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                });
            }
        }
    }

    public struct ZombieSpawner : IComponentData
    {
        public float timer;
        public float spawnInterval;
        public float randomWalkingDistanceMin;
        public float randomWalkingDistanceMax;
    }

}


