using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
    public class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new Target
            {
            });
        }
    }
    
    
}


public struct Target : Unity.Entities.IComponentData
{
    public Entity targetEntity;
}
