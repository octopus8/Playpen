using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;


namespace RTS
{
    /// <summary>
    /// System that finds targets for entities with the FindTarget component within a specified range and updates the Target component.
    /// </summary>
    partial struct FindTargetSystem : ISystem
    {
        /// <summary>
        /// Finds targets for entities with the FindTarget component within a specified range and updates the Target component.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with LocalTransform, FindTarget, and Target components.
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
            foreach ((
                         RefRO<LocalTransform> localTransform,
                         RefRW<FindTarget> findTarget,
                         RefRW<Target> target
                     )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<FindTarget>,
                         RefRW<Target>
                     >()
                    )
            {
                // Decrease the timer and check if it's time to search for a new target.
                findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.timer > 0f)
                {
                    continue;
                }

                // Perform an overlap sphere query to find potential targets within range.
                target.ValueRW.targetEntity = Entity.Null;
                findTarget.ValueRW.timer = findTarget.ValueRO.maxTimer;
                hits.Clear();
                CollisionFilter collisonFilter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1 << RTSGame.UNITS_LAYER,
                    GroupIndex = 0
                };
                if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref hits,
                        collisonFilter))
                {
                    // Iterate through the hits to find a valid target.
                    foreach (DistanceHit distanceHit in hits)
                    {
                        // Check if the entity is valid and has a Unit component.
                        // For some reason OverlapSphere can return entities that don't exist or don't have the expected component.
                        if (!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Unit>(distanceHit.Entity))
                        {
                            continue;
                        }
                        
                        // Check if the unit belongs to the target faction.
                        Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                        if (targetUnit.faction == findTarget.ValueRO.targetFaction)
                        {
                            // Set the target entity and break out of the loop.
                            target.ValueRW.targetEntity = distanceHit.Entity;
                            break;
                        }
                    }
                }
            }
        }
    }
}
