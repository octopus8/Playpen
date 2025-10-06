using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for target data. This component is added to entities that can be targeted.
    /// </summary>
    public class TargetAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Target
                {
                });
            }
        }
    }


    /// <summary>
    /// Component storing target data for an entity.
    /// </summary>
    public struct Target : IComponentData
    {
        /// <summary>Entity representing the target.</summary>
        public Entity targetEntity;
    }
}
