using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component to tag an entity as a zombie.
    /// </summary>
    public class EnemyAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Unity.Entities.Baker<EnemyAuthoring>
        {
            /// <summary>
            /// Adds the Enemy tag component to the entity.
            /// </summary>
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent<Enemy>(entity);
            }
        }
    }


    /// <summary>
    /// Tag component indicating that an entity is an enemy.
    /// </summary>
    public struct Enemy : Unity.Entities.IComponentData
    {
    }    
}

