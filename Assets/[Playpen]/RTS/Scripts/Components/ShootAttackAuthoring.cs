using Unity.Mathematics;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for shoot attack data. This component is added to units that can perform shoot attacks.
    /// </summary>
    public class ShootAttackAuthoring : MonoBehaviour
    {
        /// <summary>Time in seconds between attacks.</summary>
        [Tooltip("Time in seconds between attacks.")]
        [SerializeField] private float attackRateSeconds = 0.2f;

        /// <summary>Amount of damage dealt per attack.</summary>
        [Tooltip("Amount of damage dealt per attack.")]
        [SerializeField] private int damageAmount = 10;

        /// <summary>Maximum distance to attack targets.</summary>
        [Tooltip("Maximum distance to attack targets.")]
        [SerializeField] private int attackDistance = 7;
        
        /// <summary>Transform representing the bullet spawn point.</summary>
        [Tooltip("Transform representing the bullet spawn point.")]
        [SerializeField] private Transform bulletSpawnPoint;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Unity.Entities.Baker<ShootAttackAuthoring>
        {
            /// <summary>
            /// Adds the ShootAttack component to the entity with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack
                {
                    attackRateSeconds = authoring.attackRateSeconds,
                    damageAmount = authoring.damageAmount,
                    attackDistance = authoring.attackDistance,
                    bulletSpawnOffset = authoring.bulletSpawnPoint.localPosition,
                });
            }
        }
    }


    /// <summary>
    /// Component storing data for shoot attacks, including attack rate, damage amount, and bullet spawn offset.
    /// </summary> 
    public struct ShootAttack : Unity.Entities.IComponentData
    {
        /// <summary>Timer to track time between attacks.</summary>
        public float timer;
        
        /// <summary>Time in seconds between attacks.</summary>
        public float attackRateSeconds;
        
        /// <summary>Amount of damage dealt per attack.</summary>
        public int damageAmount;
        
        /// <summary>Maximum distance to attack targets.</summary>
        public float attackDistance;
        
        /// <summary>Offset from the unit's position to spawn bullets.</summary>
        public float3 bulletSpawnOffset;
        
        /// <summary>Event flag for when a shoot attack is triggered.</summary>
        public OnShootEvent onShootEvent;

        
        /// <summary>
        /// Event data for when a shoot attack is triggered.
        /// </summary>
        public struct OnShootEvent
        {
            /// <summary>Flag to indicate if the shoot event has been triggered.</summary>
            public bool isTriggered;
            
            /// <summary>Position from which the bullet should be spawned.</summary>
            public float3 shootPosition;
        }
    }
}
