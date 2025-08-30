using Unity.Entities;
using UnityEngine;

namespace RTS
{

    public class BulletAuthoring : MonoBehaviour
    {
        public float speed = 2;
        public int damageAmount = 5;

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
        public float speed;
        public int damageAmount;
    }
}
