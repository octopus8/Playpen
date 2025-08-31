using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace RTS
{
    partial struct ZombieSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();
            foreach (var (localTransform, spawner) in
                     SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<ZombieSpawner>
                     >())
            {
                spawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (spawner.ValueRO.timer > 0f)
                {
                    continue;
                }
                spawner.ValueRW.timer = spawner.ValueRO.spawnInterval;
                var zombie = state.EntityManager.Instantiate(entityReferences.zombiePrefabEntity);
                SystemAPI.SetComponent(zombie, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            }
        }
    }
}
