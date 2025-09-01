using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// System that moves bullets towards their targets and applies damage upon impact.
    /// </summary>
    partial struct BulletMoverSystem : ISystem
    {
        /// <summary>
        /// Moves bullets towards their target and applies damage upon impact.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRO<Bullet> bullet,
                         RefRO<Target> target,
                         Entity entity
                     )
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<Bullet>,
                         RefRO<Target>
                     >().WithEntityAccess()
                    )
            {
                // If the target entity is null, destroy the bullet entity.
                if (target.ValueRO.targetEntity == Entity.Null)
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }

                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                ShootTarget shootTarget = SystemAPI.GetComponent<ShootTarget>(target.ValueRO.targetEntity);
                float3 targetPosition = targetLocalTransform.TransformPoint(shootTarget.hitLocalPosition);

                // Store the previous distance to the target to check if we've passed it.
                float prevDistanceToTargetSq =
                    math.distancesq(localTransform.ValueRO.Position, targetPosition);

                // Move the bullet towards the target.
                float3 direction = math.normalize(targetPosition - localTransform.ValueRO.Position);
                localTransform.ValueRW.Position += direction * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;
                
                // If the bullet has reached or passed the target, apply damage and destroy the bullet.
                float distanceToTarget =
                    math.distancesq(localTransform.ValueRO.Position, targetPosition);
                float minDistanceToTarget = 0.2f;
                if (distanceToTarget < minDistanceToTarget || distanceToTarget > prevDistanceToTargetSq)
                {
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.currentHealth -= bullet.ValueRO.damageAmount;
                    targetHealth.ValueRW.onHealthChanged = true;
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}
