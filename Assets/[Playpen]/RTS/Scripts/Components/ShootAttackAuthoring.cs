using Unity.Mathematics;
using UnityEngine;


namespace RTS
{

    public class ShootAttackAuthoring : MonoBehaviour
    {
        [SerializeField] private float attackRateSeconds = 0.2f;

        [SerializeField] private int damageAmount = 10;

        [SerializeField] private int attackDistance = 7;
        
        [SerializeField] private Transform bulletSpawnPoint;

        public class Baker : Unity.Entities.Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack
                {
                    attackRateSeconds = authoring.attackRateSeconds,
                    damageAmount = authoring.damageAmount,
                    attackDistance = authoring.attackDistance,
                    bulletSpawnOffset = authoring.bulletSpawnPoint.localPosition,
                });
            }
        }
    }

    public struct ShootAttack : Unity.Entities.IComponentData
    {
        public float timer;
        public float attackRateSeconds;
        public int damageAmount;
        public float attackDistance;
        public float3 bulletSpawnOffset;
    }
}
