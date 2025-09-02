using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class MeleeAttackAuthoring : MonoBehaviour
    {
        /// <summary>Time in seconds between attacks.</summary>
        [Tooltip("Time in seconds between attacks.")]
        [SerializeField] private float attackRateSeconds = 0.2f;
        
        /// <summary>Amount of damage dealt per attack.</summary>
        [Tooltip("Amount of damage dealt per attack.")]
        [SerializeField] private int damageAmount = 10;

        /// <summary>Size of the melee attack collider.</summary>
        [Tooltip("Size of the melee attack collider.")]
        [SerializeField] private float colliderSize = 2f;
        
        class Baker : Baker<MeleeAttackAuthoring>
        {
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
    
    public struct MeleeAttack : IComponentData
    {
        public float timer;
        public float attackRateSeconds;
        public int damageAmount;
        public float colliderSize;
    }

    
}
