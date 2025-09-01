using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for health data. This component is added to units that have health.
    /// </summary>
    public class HealthAuthoring : MonoBehaviour
    {
        /// <summary>Maximum health.</summary>
        [Tooltip("Maximum health.")]
        [SerializeField] private int maxHealth = 100;

        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health
                {
                    currentHealth = authoring.maxHealth,
                    maxHealth = authoring.maxHealth,
                    onHealthChanged = true // Initialize to true to ensure health bar is set correctly at start
                });
            }
        }
    }


    public struct Health : IComponentData
    {
        /// <summary>Current health.</summary>
        public int currentHealth;
        /// <summary>Maximum health.</summary>
        public int maxHealth;
        /// <summary>Flag to indicate if health has changed.</summary>
        public bool onHealthChanged;
    }
}
