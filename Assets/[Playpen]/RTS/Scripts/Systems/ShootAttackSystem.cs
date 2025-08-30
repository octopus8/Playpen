using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferences references = SystemAPI.GetSingleton<EntityReferences>();
        foreach ((RefRO<LocalTransform> localTransform,
                     RefRW<ShootAttack> shootAttack,
                     RefRO<Target> target)
                 in SystemAPI.Query<
                     RefRO<LocalTransform>,
                     RefRW<ShootAttack>,
                     RefRO<Target>>())
        {
            // If no target, skip.
            if (target.ValueRO.targetEntity == Entity.Null)
                continue;
            
            // If still waiting for next attack, skip.
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRW.timer > 0)
            {
                continue;
            }
            
            // Reset timer.
            shootAttack.ValueRW.timer = shootAttack.ValueRO.attackRateSeconds;

            Entity bullet = state.EntityManager.Instantiate(references.bulletPrefabEntity);
            SystemAPI.SetComponent(bullet, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bullet);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;
            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bullet);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }
}
