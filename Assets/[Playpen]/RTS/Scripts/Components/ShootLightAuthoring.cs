using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class ShootLightAuthoring : MonoBehaviour
    {
        public float timer = 0.02f;

        class Baker : Baker<ShootLightAuthoring>
        {
            public override void Bake(ShootLightAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootLight
                {
                    timer = authoring.timer,
                });
            }
        }
    }

    
    public struct ShootLight : IComponentData
    {
        public float timer;
    }
}
