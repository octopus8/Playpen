using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    public class UnitMoverOverrideAuthoring : MonoBehaviour
    {
        class Baker : Baker<UnitMoverOverrideAuthoring>
        {
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

    
    public struct UnitMoverOverride : IComponentData, IEnableableComponent
    {
        public float3 targetPosition;
    }
}

