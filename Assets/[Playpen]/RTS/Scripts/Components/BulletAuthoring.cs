using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for bullet data. This component is added to bullet prefabs.
    /// </summary>
    public class BulletAuthoring : MonoBehaviour
    {
        /// <summary>Bullet speed in meters per second.</summary>
        [Tooltip("Bullet speed in meters per second.")]
        [SerializeField] private float speed = 2;
        
        /// <summary>Amount of damage the bullet deals on hit.</summary>
        [Tooltip("Amount of damage the bullet deals on hit.")]
        [SerializeField] private int damageAmount = 5;


        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<BulletAuthoring>
        {
            /// <summary>
            /// Adds the Bullet component to the entity with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Bullet
                {
                    speed = authoring.speed,
                    damageAmount = authoring.damageAmount
                });
            }
        }
    }
    
    
    /// <summary>
    /// Component storing data for bullets, including speed and damage amount.
    /// </summary>
    public struct Bullet : IComponentData
    {
        /// <summary>Bullet speed in meters per second.</summary>
        public float speed;
        
        /// <summary>Amount of damage the bullet deals on hit.</summary>
        public int damageAmount;
    }
}
