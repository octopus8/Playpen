using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for bullets.
    /// </summary>
    public class BulletAuthoring : MonoBehaviour
    {
        /// <summary>Bullet speed in meters per second.</summary>
        [Tooltip("Bullet speed in meters per second.")]
        [SerializeField] private float speed = 2;
        
        /// <summary>Amount of damage the bullet deals on hit.</summary>
        [Tooltip("Amount of damage the bullet deals on hit.")]
        [SerializeField] private int damageAmount = 5;

        
        class Baker : Baker<BulletAuthoring>
        {
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
    
    
    public struct Bullet : IComponentData
    {
        /// <summary>Bullet speed in meters per second.</summary>
        public float speed;
        /// <summary>Amount of damage the bullet deals on hit.</summary>
        public int damageAmount;
    }
}
