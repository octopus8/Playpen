using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    [SerializeField]
    private float attackRateSeconds = 0.2f;

    public class Baker : Unity.Entities.Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack
            {
                attackRateSeconds = authoring.attackRateSeconds,
            });
        }
    }
}

public struct ShootAttack : Unity.Entities.IComponentData
{
    public float timer;
    public float attackRateSeconds;
}