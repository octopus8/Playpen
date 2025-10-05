using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component to tag an entity as a zombie.
    /// </summary>
    public class EnemyAuthoring : MonoBehaviour
    {
        public class Baker : Unity.Entities.Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent<Enemy>(entity);
            }
        }
    }


    public struct Enemy : Unity.Entities.IComponentData
    {
    }    
}

