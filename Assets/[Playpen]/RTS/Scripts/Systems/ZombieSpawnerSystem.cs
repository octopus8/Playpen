using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
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
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
            CollisionFilter collisionFilter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = 1 << RTSGame.UNITS_LAYER,
                GroupIndex = 0
            };
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
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

                int nearbyZombies = 0;
                hits.Clear();
                if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, spawner.ValueRO.nearbyZombieRadius,
                        ref hits, collisionFilter))
                {
                    foreach (var hit in hits)
                    {
                        if (!SystemAPI.Exists(hit.Entity))
                        {
                            continue;
                        }
                        if (SystemAPI.HasComponent<Unit>(hit.Entity) && SystemAPI.HasComponent<Zombie>(hit.Entity))
                        {
                            ++nearbyZombies;
                        }
                    }
                }
                if (nearbyZombies >= spawner.ValueRO.maxNearbyZombies)
                {
                    continue;
                }

                
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
