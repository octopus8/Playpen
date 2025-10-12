using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for the singleton tag BuildingFriendlyHQ ECS component.
    /// </summary>
    public class BuildingFriendlyHQAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<BuildingFriendlyHQAuthoring>
        {
            /// <summary>
            /// Adds the BuildingFriendlyHQ tag component to the entity.
            /// </summary>
            public override void Bake(BuildingFriendlyHQAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingFriendlyHQ());
            }
        }
    }


    /// <summary>
    /// Singleton tag component which marks an entity as a "friendly HQ" building.
    /// </summary>
    public struct BuildingFriendlyHQ : IComponentData
    {
    }
}

