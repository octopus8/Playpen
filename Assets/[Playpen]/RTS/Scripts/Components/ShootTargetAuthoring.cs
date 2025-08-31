using Unity.Mathematics;
using UnityEngine;

public class ShootTargetAuthoring : MonoBehaviour
{
    [SerializeField] private Transform hitLocalPosition;
    
    public class Baker : Unity.Entities.Baker<ShootTargetAuthoring>
    {
        public override void Bake(ShootTargetAuthoring authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootTarget
            {
                hitLocalPosition = authoring.hitLocalPosition.localPosition
            });
        }
    }
}


public struct ShootTarget : Unity.Entities.IComponentData
{
    public float3 hitLocalPosition;
}