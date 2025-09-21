using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace RTS
{
    /// <summary>
    /// System that spawns a ShootLight entity at the position of a shoot event.
    /// This system runs in the LateSimulationSystemGroup to ensure shoot events can occur before spawning the light.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ShootLightSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntityReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with ShootAttack component.
            EntityReferences references = SystemAPI.GetSingleton<EntityReferences>();
            foreach ((
                         RefRW<ShootAttack> shootAttack,
                         Entity entity
                     )
                     in SystemAPI.Query<
                         RefRW<ShootAttack>
                     >().WithEntityAccess()
                    )
            {
                // If a shoot event is triggered, spawn a ShootLight entity at the shoot position.
                if (shootAttack.ValueRO.onShootEvent.isTriggered)
                {
                    Entity shootLightEntity = state.EntityManager.Instantiate(references.shootLightEntityPrefab);
                    SystemAPI.SetComponent(shootLightEntity,
                        LocalTransform.FromPosition(shootAttack.ValueRO.onShootEvent.shootPosition));
                }
            }
        }
    }
}
