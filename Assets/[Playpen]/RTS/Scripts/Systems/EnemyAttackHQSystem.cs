using RTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyAttackHQSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildingFriendlyHQ>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity hqEntity = SystemAPI.GetSingletonEntity<BuildingFriendlyHQ>();
        float3 hqPosition = SystemAPI.GetComponent<LocalTransform>(hqEntity).Position;

        foreach (var (enemyAttackHQ, unitMover, target) in 
                 SystemAPI.Query<RefRO<EnemyAttackHQ>, RefRW<UnitMover>, RefRO<Target>>().WithDisabled<UnitMoverOverride>())
        {
            if (target.ValueRO.targetEntity != Entity.Null)
            {
                continue;
            }
            
            unitMover.ValueRW.targetPosition = hqPosition;
            
        }

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
