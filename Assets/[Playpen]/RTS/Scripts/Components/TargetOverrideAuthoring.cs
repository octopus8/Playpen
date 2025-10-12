using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for the TargetOverride ECS component.
    /// Overriding the target is used to override auto targeting, such as when a player manually selects a target for a unit to attack.
    /// </summary>
    public class TargetOverrideAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<TargetOverrideAuthoring>
        {
            public override void Bake(TargetOverrideAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetOverride
                {
                });
            }
        }
    }
    
    
    /// <summary>
    /// Component which allows overriding the target entity for systems that use it.
    /// </summary>
    public struct TargetOverride : IComponentData
    {
        public Entity targetEntity;
    }
}
