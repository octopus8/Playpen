using Unity.Burst;
using Unity.Entities;

namespace RTS
{

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthDeadTestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
            {
                if (health.ValueRO.currentHealth <= 0)
                {
                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }
    }
}
