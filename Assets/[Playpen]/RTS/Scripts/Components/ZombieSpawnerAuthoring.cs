using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for zombie spawner data. This component is added to entities that spawn zombies.
    /// </summary>
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        /// <summary>Interval in seconds between spawns.</summary>
        [Tooltip("Interval in seconds between spawns.")]
        [SerializeField] private float spawnInterval = 5f;
        
        /// <summary>Minimum random walking distance for spawned zombies.</summary>
        [Tooltip("Minimum random walking distance for spawned zombies.")]
        [SerializeField] private float randomWalkingDistanceMin;
        
        /// <summary>Maximum random walking distance for spawned zombies.</summary>
        [Tooltip("Maximum random walking distance for spawned zombies.")]
        [SerializeField] private float randomWalkingDistanceMax;

        
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
        /// <summary>Timer counting up to the next spawn.</summary>
        public float timer;
        /// <summary>Interval in seconds between spawns.</summary>
        public float spawnInterval;
        /// <summary>Minimum random walking distance for spawned zombies.</summary>
        public float randomWalkingDistanceMin;
        /// <summary>Maximum random walking distance for spawned zombies.</summary>
        public float randomWalkingDistanceMax;
    }
}


