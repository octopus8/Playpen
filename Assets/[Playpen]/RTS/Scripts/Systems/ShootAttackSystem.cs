using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{
partial struct ShootAttackSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EntityPrefabSet>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntityPrefabSet entitiesReferences = SystemAPI.GetSingleton<EntityPrefabSet>();

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target,
            RefRW<UnitMover> unitMover,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>,
                RefRW<UnitMover>>().WithDisabled<UnitMoverOverride>().WithEntityAccess()) {

            if (target.ValueRO.targetEntity == Entity.Null) {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance) {
                // Too far, move closer
                unitMover.ValueRW.destination = targetLocalTransform.Position;
                continue;
            } else {
                // Close enough, stop moving and attack
                unitMover.ValueRW.destination = localTransform.ValueRO.Position;
            }

            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);

            quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
            localTransform.ValueRW.Rotation =
                math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
        }

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW <ShootAttack> shootAttack,
            RefRO<Target> target,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>>().WithEntityAccess()) {

            if (target.ValueRO.targetEntity == Entity.Null) {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance) {
                // Target is too far
                continue;
            }

            if (SystemAPI.HasComponent<UnitMoverOverride>(entity) && SystemAPI.IsComponentEnabled<UnitMoverOverride>(entity)) {
                // Move override is active
                continue;
            }

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f) {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.attackRateSeconds;

            if (SystemAPI.HasComponent<TargetOverride>(target.ValueRO.targetEntity)) {
                RefRW<TargetOverride> enemyTargetOverride = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);
                if (enemyTargetOverride.ValueRO.targetEntity == Entity.Null) {
                    enemyTargetOverride.ValueRW.targetEntity = entity;
                }
            }

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletEntityPrefab);
            float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnOffset);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

            shootAttack.ValueRW.onShootEvent.isTriggered = true;
            shootAttack.ValueRW.onShootEvent.shootPosition = bulletSpawnWorldPosition;
        }
    }

}    
}
