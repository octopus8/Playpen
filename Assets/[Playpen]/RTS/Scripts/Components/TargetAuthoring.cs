using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for target data. This component is added to entities that can be targeted.
    /// </summary>
    public class TargetAuthoring : MonoBehaviour
    {
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


    public struct Target : IComponentData
    {
        /// <summary>Entity representing the target.</summary>
        public Entity targetEntity;
    }
}
