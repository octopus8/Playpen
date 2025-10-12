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
        
        /// <summary>Maximum number of nearby zombies allowed before stopping spawning.</summary>
        [Tooltip("Maximum number of nearby zombies allowed before stopping spawning.")]
        [SerializeField] private int maxNeabyZombies = 5;
        
        /// <summary>Radius to check for nearby zombies.</summary>
        [Tooltip("Radius to check for nearby zombies.")]
        [SerializeField] private float nearbyZombieRadius = 10f;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<ZombieSpawnerAuthoring>
        {
            /// <summary>
            /// Adds the ZombieSpawner component to the entity with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ZombieSpawner
                {
                    spawnInterval = authoring.spawnInterval,
                    randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                    randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                    maxNearbyZombies = authoring.maxNeabyZombies,
                    nearbyZombieRadius = authoring.nearbyZombieRadius,
                });
            }
        }
    }

    
    /// <summary>
    /// Component storing data for zombie spawners, including spawn interval and random walking distance for spawned zombies.
    /// </summary>
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

        /// <summary>Maximum number of nearby zombies allowed before stopping spawning.</summary>
        public int maxNearbyZombies;
        
        /// <summary>Radius to check for nearby zombies.</summary>
        public float nearbyZombieRadius;
    }
}


