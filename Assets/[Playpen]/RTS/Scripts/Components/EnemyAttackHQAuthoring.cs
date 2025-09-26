using Unity.Entities;
using UnityEngine;


namespace RTS
{
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

