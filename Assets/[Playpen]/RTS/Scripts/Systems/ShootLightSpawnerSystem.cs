using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace RTS
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ShootLightSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
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
                if (shootAttack.ValueRO.onShootEvent.isTriggered)
                {
                    Entity shootLightEntity = state.EntityManager.Instantiate(references.shootLightEntity);
                    SystemAPI.SetComponent(shootLightEntity,
                        LocalTransform.FromPosition(shootAttack.ValueRO.onShootEvent.shootPosition));
                }
            }
        }
    }
}
