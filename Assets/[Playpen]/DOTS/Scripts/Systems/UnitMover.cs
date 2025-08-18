using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct UnitMover : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<MoveSpeedDOTS> moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedDOTS>>())
        {
            float3 targetPosition = localTransform.ValueRO.Position + new float3(10, 0, 0);
            float3 moveDirection = math.normalize(targetPosition - localTransform.ValueRO.Position);
            localTransform.ValueRW.Rotation = quaternion.LookRotation(moveDirection, math.up());
            localTransform.ValueRW.Position += moveDirection * moveSpeed.ValueRO.value * SystemAPI.Time.DeltaTime;
        }
   }
}
