using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for melee attack behavior.
    /// </summary>
    public class MeleeAttackAuthoring : MonoBehaviour
    {
        /// <summary>Time in seconds between attacks.</summary>
        [Tooltip("Time in seconds between attacks.")]
        [SerializeField] private float attackRateSeconds = 0.2f;
        
        /// <summary>Amount of damage dealt per attack.</summary>
        [Tooltip("Amount of damage dealt per attack.")]
        [SerializeField] private int damageAmount = 10;

        /// <summary>Size of this unit's collider's radius.</summary>
        [Tooltip("Size of this unit's collider's radius.")]
        [SerializeField] private float colliderSize = 2f;
        
        
        /// <summary>
        /// Baker class to convert the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<MeleeAttackAuthoring>
        {
            /// <summary>
            /// Converts the authoring component to an ECS component with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(MeleeAttackAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MeleeAttack
                {
                    attackRateSeconds = authoring.attackRateSeconds,
                    damageAmount = authoring.damageAmount,
                    colliderSize = authoring.colliderSize
                });
            }
        }
    }
    
    
    /// <summary>
    /// ECS component representing melee attack properties.
    /// </summary>
    public struct MeleeAttack : IComponentData
    {
        /// <summary>Timer to track time between attacks.</summary>
        public float timer;
        
        /// <summary>Time in seconds between attacks.</summary>
        public float attackRateSeconds;
        
        /// <summary>Amount of damage dealt per attack.</summary>
        public int damageAmount;
        
        /// <summary>Size of this unit's collider's radius.</summary>
        public float colliderSize;
        
        /// <summary> Event flag to indicate if currently attacking the target. </summary>
        public bool onAttackTarget;
    }
}
