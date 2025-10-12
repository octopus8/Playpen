using Unity.Burst;
using Unity.Entities;

namespace RTS
{
    /// <summary>
    /// System that destroys ShootLight entities after their timer expires.
    /// </summary>
    partial struct ShootLightDestroySystem : ISystem
    {
        
        /// <summary>
        /// Destroys ShootLight entities when their timer expires.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with ShootLight component.
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            foreach ((
                         RefRW<ShootLight> shootLight,
                         Entity entity
                     )
                     in SystemAPI.Query<
                         RefRW<ShootLight>
                     >().WithEntityAccess()
                    )
            {
                // Decrease timer and destroy entity if timer has expired.
                shootLight.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (shootLight.ValueRO.timer <= 0f)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}
