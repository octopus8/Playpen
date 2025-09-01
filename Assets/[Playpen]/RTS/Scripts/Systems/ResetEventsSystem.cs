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
}
