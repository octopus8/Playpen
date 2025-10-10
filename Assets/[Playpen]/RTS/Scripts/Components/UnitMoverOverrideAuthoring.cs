using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for unit movement override data. This component can be added to units to allow temporary movement overrides.
    /// This component starts disabled and is enabled to set a temporary override destination for the unit.
    /// </summary> 
    public class UnitMoverOverrideAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<UnitMoverOverrideAuthoring>
        {
            /// <summary>
            /// Adds the UnitMoverOverride component to the entity.
            /// The component is added in a disabled state.
            /// </summary>
            public override void Bake(UnitMoverOverrideAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMoverOverride
                {
                });
                SetComponentEnabled<UnitMoverOverride>(entity, false);
            }
        }
    }


    /// <summary>
    /// Enableable component storing temporary movement override data for a unit.
    /// </summary>
    public struct UnitMoverOverride : IComponentData, IEnableableComponent
    {
        /// <summary> Destination to temporarily override the unit's movement to. </summary>
        public float3 overrideDestination;
    }
}

