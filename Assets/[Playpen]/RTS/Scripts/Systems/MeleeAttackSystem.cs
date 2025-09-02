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
    partial struct MeleeAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
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
                     >())
            {
                // If no target, skip.
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;
                
                // If target out of range, skip.
                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                float distancesq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
                float meleeRangeSq = 2f;
                bool isInMeleeRange = distancesq <= meleeRangeSq;

                bool isTouchingTarget = false;
                if (!isInMeleeRange)
                {
                    float3 directionToTarget =
                        math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);
                    float extraDistanceToTestRaycast = 0.4f;
                    RaycastInput raycastInput = new RaycastInput
                    {
                        Start = localTransform.ValueRO.Position,
                        End = localTransform.ValueRO.Position + directionToTarget * (meleeAttack.ValueRO.colliderSize + extraDistanceToTestRaycast),
                        Filter = CollisionFilter.Default
                    };
                    raycastHits.Clear();
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
                
                if (!isInMeleeRange && !isTouchingTarget)
                {
                    unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                    continue;
                }
                
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                
                // If still waiting for next attack, skip.
                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (meleeAttack.ValueRO.timer > 0)
                {
                    continue;
                }
                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.attackRateSeconds;
                
                if (SystemAPI.HasComponent<Health>(target.ValueRO.targetEntity))
                {
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.currentHealth -= meleeAttack.ValueRO.damageAmount;
                    targetHealth.ValueRW.onHealthChanged = true;
                }
            }
        }
    }
}

