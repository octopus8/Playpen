using Unity.Entities;
using UnityEngine;


namespace RTS
{

    public class HealthAuthoring : MonoBehaviour
    {
        public int maxHealth = 100;

        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health
                {
                    currentHealth = authoring.maxHealth,
                    maxHealth = authoring.maxHealth
                });
            }
        }
    }


    public struct Health : IComponentData
    {
        public int currentHealth;
        public int maxHealth;
    }
}
