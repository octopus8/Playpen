using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct BulletMoverSystem : ISystem
{
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
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                ecb.DestroyEntity(entity);
                continue;
            }
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float prevDistanceToTargetSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
            
            float3 direction = math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);
            localTransform.ValueRW.Position += direction * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;
            float distanceToTarget = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
            float minDistanceToTarget = 0.2f;
            if (distanceToTarget < minDistanceToTarget || distanceToTarget > prevDistanceToTargetSq)
            {                
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                ecb.DestroyEntity(entity);                
            }
        }
    }
}
