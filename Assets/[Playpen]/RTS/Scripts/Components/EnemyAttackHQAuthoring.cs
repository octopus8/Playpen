using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for the EnemyAttackHQ tag component.
    /// Can be added to enemy units to enable them to attack the player's HQ.
    /// </summary>
    public class EnemyAttackHQAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<EnemyAttackHQAuthoring>
        {
            /// <summary>
            /// Adds the EnemyAttackHQ tag component to the entity.
            /// </summary>
            public override void Bake(EnemyAttackHQAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyAttackHQ());
            }
        }
    }
    
    
    /// <summary>
    /// Tag component indicating that an enemy unit can attack the player's HQ.
    /// </summary>
    public struct EnemyAttackHQ : IComponentData
    {
    }
}

