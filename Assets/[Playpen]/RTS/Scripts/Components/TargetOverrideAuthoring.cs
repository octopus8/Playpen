using Unity.Entities;
using UnityEngine;

namespace RTS
{

    public class TargetOverrideAuthoring : MonoBehaviour
    {
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
    
    public struct TargetOverride : IComponentData
    {
        public Entity targetEntity;
    }
    
}
