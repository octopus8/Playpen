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

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Baker<HealthAuthoring>
        {
            /// <summary>
            /// Adds the Health component to the entity with the specified parameters from the authoring component.
            /// </summary>
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


    /// <summary>
    /// Component storing data for health, including current and maximum health.
    /// Also includes event flags for health changes and death.
    /// </summary>
    public struct Health : IComponentData
    {
        /// <summary>Current health.</summary>
        public int currentHealth;
        
        /// <summary>Maximum health.</summary>
        public int maxHealth;
        
        /// <summary>Event flag to indicate health has changed.</summary>
        public bool onHealthChanged;

        /// <summary>Event flag to indicate death.</summary>
        public bool onDead;
    }
}
