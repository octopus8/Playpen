using Unity.Entities;
using UnityEngine;

namespace RTS
{

    /// <summary>
    /// Authoring component for health bar data. This component is added to units that have a health bar.
    /// </summary>
    public class HealthBarAuthoring : MonoBehaviour
    {
        /// <summary>GameObject representing the visual part of the health bar.</summary>
        [Tooltip("GameObject representing the visual part of the health bar.")]
        [SerializeField] private GameObject barVisual;
        
        /// <summary>GameObject representing the health component.</summary>
        [Tooltip("GameObject representing the health component.")]
        [SerializeField] private HealthAuthoring health;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<HealthBarAuthoring>
        {
            /// <summary>
            /// Adds the HealthBar component to the entity with references to the bar visual and health entities.
            /// </summary>
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

    
    /// <summary>
    /// Component storing references for the health bar, including the visual entity and the health entity.
    /// </summary>
    public struct HealthBar : IComponentData
    {
        /// <summary>Entity representing the visual part of the health bar.</summary>
        public Entity barVisualEntity;
        
        /// <summary>Entity representing the health component.</summary>
        public Entity healthEntity;
    }
}