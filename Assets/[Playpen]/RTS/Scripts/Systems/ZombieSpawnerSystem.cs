using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace RTS
{
    /// <summary>
    /// System that spawns zombie entities at the position of ZombieSpawner components.
    /// </summary>
    partial struct ZombieSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntityReferences>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with ZombieSpawner component.
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();
            foreach (var (localTransform, spawner) in
                     SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<ZombieSpawner>
                     >())
            {
                if (spawner.ValueRO.currentSpawnedZombies >= spawner.ValueRO.maxSpawnedZombies)
                {
                    continue;
                }
                
                spawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (spawner.ValueRO.timer > 0f)
                {
                    continue;
                }
                spawner.ValueRW.timer = spawner.ValueRO.spawnInterval;
                spawner.ValueRW.currentSpawnedZombies++;
                var zombie = state.EntityManager.Instantiate(entityReferences.zombieEntity);
                SystemAPI.SetComponent(zombie, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                
                ecb.AddComponent(zombie, new RandomWalking
                {
                    targetPosition = localTransform.ValueRO.Position,
                    originPosition = localTransform.ValueRO.Position,
                    distanceMin = spawner.ValueRO.randomWalkingDistanceMin,
                    distanceMax = spawner.ValueRO.randomWalkingDistanceMax,
                    random = new Unity.Mathematics.Random((uint)zombie.Index)
                });
            }
        }
    }
}
