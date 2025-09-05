using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


namespace RTS
{
    /// <summary>
    /// System that resets the target entity in the Target component if the target entity has been destroyed.
    /// This system runs at the beginning of the SimulationSystemGroup to ensure targets are valid before other systems process them.
    /// </summary> 
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct ResetTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with Target component.
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
                if (!SystemAPI.Exists(target.ValueRO.targetEntity) ||
                    !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
                {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
            
            foreach (RefRW<TargetOverride> targetOverride in SystemAPI.Query<RefRW<TargetOverride>>())
            {
                // If no target, skip.
                if (targetOverride.ValueRO.targetEntity == Entity.Null)
                {
                    continue;
                }

                // If target entity has been destroyed, reset target to null.
                // Note: We also check if the entity has a LocalTransform component to detect
                // entities that might not have been fully destroyed (complete destruction seems to occur in "Parent System" due to "cleanup components").
                if (!SystemAPI.Exists(targetOverride.ValueRO.targetEntity) ||
                    !SystemAPI.HasComponent<LocalTransform>(targetOverride.ValueRO.targetEntity))
                {
                    targetOverride.ValueRW.targetEntity = Entity.Null;
                }
            }
        }
    }
}
