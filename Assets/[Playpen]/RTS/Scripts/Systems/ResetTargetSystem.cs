using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            // If no target, skip.
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }
            
            // If target entity has been destroyed, reset target to null.
            // Note: We also check if the entity has a LocalTransform component to detect
            // entities that might not have been fully destroyed (complete destruction seems to occur in "Parent System" due to "cleanup components").
            if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
            {
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
    }
}
