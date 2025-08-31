using Unity.Entities;
using UnityEngine;

namespace RTS
{



    public class HealthBarAuthoring : MonoBehaviour
    {
        public GameObject barVisual;
        public GameObject health;

        class Baker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthBar
                {
                    barVisualEntity = GetEntity(authoring.barVisual, TransformUsageFlags.NonUniformScale),
                    healthEntity = GetEntity(authoring.health, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct HealthBar : IComponentData
    {
        public Entity barVisualEntity;
        public Entity healthEntity;
    }
}