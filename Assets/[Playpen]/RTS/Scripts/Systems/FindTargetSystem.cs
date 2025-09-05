using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


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
                             RefRW<Target> target,
                             RefRW<TargetOverride> targetOverride
                     )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<FindTarget>,
                         RefRW<Target>,
                         RefRW<TargetOverride>
                     >()
                    )
            {
                // Decrease the timer and check if it's time to search for a new target.
                findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.timer > 0f)
                {
                    continue;
                }
                findTarget.ValueRW.timer = findTarget.ValueRO.maxTimer;
                
                if (targetOverride.ValueRO.targetEntity != Entity.Null)
                {
                    target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                    continue;
                }

                // Perform an overlap sphere query to find potential targets within range.
                hits.Clear();
                CollisionFilter collisonFilter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1 << RTSGame.UNITS_LAYER,
                    GroupIndex = 0
                };
                
                Entity closestTargetEntity = Entity.Null;
                float closestDistance = float.MaxValue;
                float currentTargetDistanceOffset = 2f;
                if (target.ValueRO.targetEntity != Entity.Null)
                {
                    closestTargetEntity = target.ValueRO.targetEntity;
                    LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(closestTargetEntity);
                    closestDistance = math.distance(localTransform.ValueRO.Position, targetTransform.Position);
                }
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
                            if (closestTargetEntity == Entity.Null)
                            {
                                closestTargetEntity = distanceHit.Entity;
                                closestDistance = distanceHit.Distance;
                            }
                            else
                            {
                                if (distanceHit.Distance + currentTargetDistanceOffset < closestDistance)
                                {
                                    closestTargetEntity = distanceHit.Entity;
                                    closestDistance = distanceHit.Distance;
                                }
                            }
                        }
                    } 
                }

                if (closestTargetEntity != Entity.Null)
                {
                    target.ValueRW.targetEntity = closestTargetEntity;
                }
            }
        }
    }
}
