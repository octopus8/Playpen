using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{
    /// <summary>
    /// System that handles shooting attacks for entities with ShootAttack component.
    /// It rotates the entity towards its target, moves it within attack range, and spawns bullets when attacking.
    /// This system requires the EntityPrefabSet singleton to be present for the system to update.
    /// </summary>
    partial struct ShootAttackSystem : ISystem
    {

        /// <summary>
        /// OnCreate is called when the system is created. It requires the EntityPrefabSet singleton to be present for the system to update.
        /// </summary>
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EntityPrefabSet>();
        }

        
        /// <summary>
        /// OnUpdate is called every frame to process entities with ShootAttack component.
        /// It rotates the entity towards its target, moves it within attack range, and spawns bullets when attacking.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            // Iterate over all entities with LocalTransform, ShootAttack, Target, and UnitMover components.
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

                // If no target, skip.
                if (target.ValueRO.targetEntity == Entity.Null) {
                    continue;
                }

                // If target is too far, move towards it.
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance) {
                    unitMover.ValueRW.destination = targetLocalTransform.Position;
                    continue;
                }
                
                //  Otherwise, close enough, set destination to current position to stop moving.
                else {
                    unitMover.ValueRW.destination = localTransform.ValueRO.Position;
                }

                // Rotate towards target.
                float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);
                quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            }

            // Iterate over all entities with LocalTransform, ShootAttack, and Target components.
            foreach ((
                RefRW<LocalTransform> localTransform,
                RefRW <ShootAttack> shootAttack,
                RefRO<Target> target,
                Entity entity)
                in SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRW<ShootAttack>,
                    RefRO<Target>>().WithEntityAccess()) {

                // If no target, skip.
                if (target.ValueRO.targetEntity == Entity.Null) {
                    continue;
                }

                // If target is out of range, skip.
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance) {
                    continue;
                }
                
                // If there's a UnitMoverOverride component and it's enabled, skip.
                if (SystemAPI.HasComponent<UnitMoverOverride>(entity) && SystemAPI.IsComponentEnabled<UnitMoverOverride>(entity)) {
                    continue;
                }

                // If the timer is still running, decrease it and skip.
                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRO.timer > 0f) {
                    continue;
                }
                
                // Reset the timer.
                shootAttack.ValueRW.timer = shootAttack.ValueRO.attackRateSeconds;

                // If the target has a TargetOverride component and its targetEntity is null, set it to this entity.
                if (SystemAPI.HasComponent<TargetOverride>(target.ValueRO.targetEntity)) {
                    RefRW<TargetOverride> enemyTargetOverride = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);
                    if (enemyTargetOverride.ValueRO.targetEntity == Entity.Null) {
                        enemyTargetOverride.ValueRW.targetEntity = entity;
                    }
                }

                // Spawn a bullet entity at the shoot position.
                Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletEntityPrefab);
                float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnOffset);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

                // Set the bullet's damage amount and target entity.
                RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;
                RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

                // Trigger the onShootEvent and set the shoot position.
                shootAttack.ValueRW.onShootEvent.isTriggered = true;
                shootAttack.ValueRW.onShootEvent.shootPosition = bulletSpawnWorldPosition;
            }
        }
    }    
}
