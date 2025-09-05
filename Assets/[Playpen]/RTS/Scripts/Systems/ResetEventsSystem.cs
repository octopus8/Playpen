using Unity.Burst;
using Unity.Entities;

namespace RTS
{
    /// <summary>
    /// System that resets event flags at the end of each frame.
    /// This system runs in the LateSimulationSystemGroup and set OrderLast to true to ensure all events are processed before resetting.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    partial struct ResetEventsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ResetSelectedEventsJob().ScheduleParallel();
            new ResetHealthEventsJob().ScheduleParallel();
            new ResetShootAttackEventsJob().ScheduleParallel();
        }

        
        public void OnCreateNoJobs(ref SystemState state)
        {
            // Reset event flags for selected events.
            foreach (var selected in SystemAPI.Query<RefRW<Selected>>())
            {
                selected.ValueRW.onSelected = false;
                selected.ValueRW.onDeselected = false;
            }
            // Reset health change event flag.
            foreach (var health in SystemAPI.Query<RefRW<Health>>())
            {
                health.ValueRW.onHealthChanged = false;
            }
            // Reset shoot event flag.
            foreach (var shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
            {
                shootAttack.ValueRW.onShootEvent.isTriggered = false;
            }
        }
    }
 
    [BurstCompile]
    public partial struct ResetShootAttackEventsJob : IJobEntity
    {
        public void Execute(ref ShootAttack shootAttack)
        {
            shootAttack.onShootEvent.isTriggered = false;
        }
    }
    
    [BurstCompile]
    public partial struct ResetHealthEventsJob : IJobEntity
    {
        public void Execute(ref Health health)
        {
            health.onHealthChanged = false;
        }
    }
    
    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct ResetSelectedEventsJob : IJobEntity
    {
        public void Execute(ref Selected selected)
        {
            selected.onSelected = false;
            selected.onDeselected = false;
        }
    }
    
}
