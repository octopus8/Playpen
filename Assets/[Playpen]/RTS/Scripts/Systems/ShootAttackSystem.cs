using Unity.Burst;
using Unity.Entities;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>())
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

            RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
            int damageAmount = 1;
            targetHealth.ValueRW.healthAmount -= damageAmount;
        }
    }
}
