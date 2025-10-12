using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Tag authoring component for the SetupUnitMoverDefaultPosition ECS component.
    /// </summary>
    public class SetupUnitMoverDefaultPositionAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<SetupUnitMoverDefaultPositionAuthoring>
        {
            /// <summary>
            /// Adds the SetupUnitMoverDefaultPosition tag component to the entity.
            /// </summary>
            public override void Bake(SetupUnitMoverDefaultPositionAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SetupUnitMoverDefaultPosition
                {
                });
            }
        }
    }
    
    
    /// <summary>
    /// Tag component which marks an entity to have its UnitMover's default position set up.
    /// </summary>
    public struct SetupUnitMoverDefaultPosition : IComponentData
    {
    }
}

