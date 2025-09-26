using Unity.Burst;
using Unity.Entities;

namespace RTS
{
    /// <summary>
    /// System that destroys entities with Health component when their health reaches zero or below.
    /// This system runs in the LateSimulationSystemGroup to ensure health changes are processed before checking for death.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthDeadTestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with Health component.
            EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (health, entity) in SystemAPI.Query<RefRW<Health>>().WithEntityAccess())
            {
                // If health is zero or below, destroy the entity.
                if (health.ValueRO.currentHealth <= 0)
                {
                    health.ValueRW.onDead = true;
                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }
    }
}
