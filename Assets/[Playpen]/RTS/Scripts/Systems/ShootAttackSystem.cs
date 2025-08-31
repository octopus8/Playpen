using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace RTS
{

    partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityReferences references = SystemAPI.GetSingleton<EntityReferences>();
            foreach ((RefRW<LocalTransform> localTransform,
                         RefRW<ShootAttack> shootAttack,
                         RefRO<Target> target,
                         RefRW<UnitMover> unitMover
                     )
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<ShootAttack>,
                         RefRO<Target>,
                         RefRW<UnitMover>
                     >())
            {
                // If no target, skip.
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;

                // Rotate towards target.
                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                float3 directionToTarget =
                    math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);
                quaternion targetRotation = quaternion.LookRotationSafe(directionToTarget, math.up());
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation,
                    SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

                // If target out of range, move towards it.
                float distance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
                if (distance > shootAttack.ValueRO.attackDistance)
                {
                    unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                    continue;
                }
                // Within range, stop moving.
                else
                {
                    unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                }

                // If still waiting for next attack, skip.
                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRW.timer > 0)
                {
                    continue;
                }

                // Reset timer.
                shootAttack.ValueRW.timer = shootAttack.ValueRO.attackRateSeconds;

                Entity bullet = state.EntityManager.Instantiate(references.bulletPrefabEntity);
                
                float3 bulletSpawnPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnOffset);
                SystemAPI.SetComponent(bullet, LocalTransform.FromPosition(bulletSpawnPosition));
                
                RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bullet);
                bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;
                RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bullet);
                bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
            }
        }
    }
}
