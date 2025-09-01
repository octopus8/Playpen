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
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
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
