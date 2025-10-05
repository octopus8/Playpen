using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for the EnemyAttackHQ tag component.
    /// Can be added to enemy unit prefabs to enable them to attack the player's HQ.
    /// </summary>
    public class EnemyAttackHQAuthoring : MonoBehaviour
    {

        class Baker : Baker<EnemyAttackHQAuthoring>
        {
            public override void Bake(EnemyAttackHQAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyAttackHQ());
            }
        }
    }
    
    public struct EnemyAttackHQ : IComponentData
    {
    }
}

