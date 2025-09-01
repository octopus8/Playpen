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
        /// <summary>Time in seconds the light stays active after shooting.</summary>
        public float timer;
    }
}
