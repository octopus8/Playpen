using UnityEngine;

public class DOTSShootAttack : MonoBehaviour
{
    [SerializeField]
    private float attackRateSeconds = 0.2f;

    public class Baker : Unity.Entities.Baker<DOTSShootAttack>
    {
        public override void Bake(DOTSShootAttack authoring)
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