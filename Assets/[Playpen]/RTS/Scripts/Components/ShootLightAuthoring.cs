using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for shoot light data. This component is added to shoot light prefabs.
    /// </summary>
    public class ShootLightAuthoring : MonoBehaviour
    {
        /// <summary>Time in seconds the light stays active after shooting.</summary>
        [Tooltip("Time in seconds the light stays active after shooting.")]
        [SerializeField] private float timer = 0.02f;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<ShootLightAuthoring>
        {
            /// <summary>
            /// Adds the ShootLight component to the entity with the specified parameters from the authoring component.
            /// </summary>
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

    
    /// <summary>
    /// Component storing data for shoot lights, including the time the light stays active after shooting.
    /// </summary>
    public struct ShootLight : IComponentData
    {
        /// <summary>Time in seconds the light stays active after shooting.</summary>
        public float timer;
    }
}
