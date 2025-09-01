using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace RTS
{
    /// <summary>
    /// Authoring component for random walking behavior. This component is added to units that should walk randomly within a certain range (e.g., zombies).
    /// </summary>
    public class RandomWalkingAuthoring : MonoBehaviour
    {
        /// <summary>Target position for random walking.</summary>
        [Tooltip("Target position for random walking.")]
        [SerializeField] private float3 targetPosition;
        
        /// <summary>Origin position for random walking.</summary>
        [Tooltip("Origin position for random walking.")] private float3 originPosition;
        
        /// <summary>Minimum distance from the origin to walk.</summary>
        [Tooltip("Minimum distance from the origin to walk.")]
        [SerializeField] private float distanceMin = 5f;
        
        /// <summary>Maximum distance from the origin to walk.</summary>
        [Tooltip("Maximum distance from the origin to walk.")]
        [SerializeField] private float distanceMax = 10f;
        
        /// <summary>Random seed for generating random positions.</summary>
        [Tooltip("Random seed for generating random positions.")]
        [SerializeField] private uint randomSeed = 1;

        
        class Baker : Baker<RandomWalkingAuthoring>
        {
            public override void Bake(RandomWalkingAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RandomWalking
                {
                    targetPosition = authoring.targetPosition,
                    originPosition = authoring.originPosition,
                    distanceMin = authoring.distanceMin,
                    distanceMax = authoring.distanceMax,
                    random = new Random(authoring.randomSeed)
                });
            }
        }
    }
    
    
    public struct RandomWalking : IComponentData
    {
        /// <summary>Target position for random walking.</summary>
        public float3 targetPosition;
        /// <summary>Origin position for random walking.</summary>
        public float3 originPosition;
        /// <summary>Minimum distance from the origin to walk.</summary>
        public float distanceMin;
        /// <summary>Maximum distance from the origin to walk.</summary>
        public float distanceMax;
        /// <summary>Random seed for generating random positions.</summary>
        public Random random;
    }
}
