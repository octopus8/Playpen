using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace RTS
{
    /// <summary>
    /// System that handles melee attacks for entities with MeleeAttack component.
    /// It moves the entity within attack range and applies damage to the target when attacking.
    /// This system requires the PhysicsWorldSingleton to be present for the system to update.
    /// </summary>
    partial struct MeleeAttackSystem : ISystem
    {
        /// <summary>
        /// OnCreate is called when the system is created. It requires the PhysicsWorldSingleton to be present for the system to update.
        /// </summary> 
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        /// <summary>
        /// OnUpdate is called every frame to process entities with MeleeAttack component.
        /// It moves the entity within attack range and applies damage to the target when attacking.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with LocalTransform, MeleeAttack, Target, and UnitMover components.
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
            NativeList<RaycastHit> raycastHits = new NativeList<RaycastHit>(Allocator.Temp);
            foreach (var (
                         localTransform, 
                         meleeAttack, 
                         target,
                         unitMover
                         ) in
                     SystemAPI.Query<
                         RefRO<LocalTransform>, 
                         RefRW<MeleeAttack>, 
                         RefRO<Target>,
                         RefRW<UnitMover>
                     >().WithDisabled<UnitMoverOverride>())
            {
                // If no target, skip.
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;

                // Get the squared distance to the target.
                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                float distancesq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
                
                // If not in melee range, move towards target.
                float meleeRangeSq = 2f;
                bool isInMeleeRange = distancesq <= meleeRangeSq;
                bool isTouchingTarget = false;
                if (!isInMeleeRange)
                {
                    float3 directionToTarget =
                        math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);
                    float extraDistanceToTestRaycast = 0.4f;
                    float3 startPosition = localTransform.ValueRO.Position;
                    startPosition[1] += 1f;
                    RaycastInput raycastInput = new RaycastInput
                    {
                        Start = startPosition,
                        End = startPosition + directionToTarget * (meleeAttack.ValueRO.colliderSize + extraDistanceToTestRaycast),
                        Filter = CollisionFilter.Default
                    };
                    raycastHits.Clear();
                    float rayLength = 5.0f;// (meleeAttack.ValueRO.colliderSize + extraDistanceToTestRaycast);
                    Debug.DrawRay(startPosition, directionToTarget * rayLength, Color.red, 0.1f, false);
                    if (collisionWorld.CastRay(raycastInput, ref raycastHits))
                    {
                        foreach (RaycastHit hit in raycastHits)
                        {
                            if (hit.Entity == target.ValueRO.targetEntity)
                            {
                                isTouchingTarget = true;
                                break;
                            }
                            
                        }
                    }
                }
                
                // If not in melee range and not touching target, move towards target.
                if (!isInMeleeRange && !isTouchingTarget)
                {
                    unitMover.ValueRW.destination = targetLocalTransform.Position;
                }
                
                // Otherwise, don't move and attack.
                else
                {
                    unitMover.ValueRW.destination = localTransform.ValueRO.Position;
                
                    // If still waiting for next attack, skip.
                    meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                    if (meleeAttack.ValueRO.timer > 0)
                    {
                        continue;
                    }
                    
                    // Reset the timer and apply damage to the target.
                    meleeAttack.ValueRW.timer = meleeAttack.ValueRO.attackRateSeconds;
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.currentHealth -= meleeAttack.ValueRO.damageAmount;
                    targetHealth.ValueRW.onHealthChanged = true;
                    meleeAttack.ValueRW.onAttackTarget = true;
                }
            }
        }
    }
}

