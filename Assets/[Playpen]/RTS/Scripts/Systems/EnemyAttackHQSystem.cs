using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace RTS
{
    /// <summary>
    /// System that makes enemy units with the EnemyAttackHQ component move towards the friendly HQ.
    /// </summary>
    partial struct EnemyAttackHQSystem : ISystem
    {
        /// <summary>
        /// OnCreate is called when the system is created.
        /// It requires the BuildingFriendlyHQ component to be present in the world for the system to update.
        /// </summary>
        /// <param name="state"></param>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingFriendlyHQ>();
        }

    
        /// <summary>
        /// OnUpdate is called every frame the system is enabled.
        /// It makes enemy units with the EnemyAttackHQ component move towards the friendly HQ if they
        /// do not have a target assigned.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get the entity that has the BuildingFriendlyHQ component.
            Entity hqEntity = SystemAPI.GetSingletonEntity<BuildingFriendlyHQ>();
        
            // Get the position of the friendly HQ.
            float3 hqPosition = SystemAPI.GetComponent<LocalTransform>(hqEntity).Position;

            // Iterate over all entities with EnemyAttackHQ, UnitMover, and Target components
            // that do not have the UnitMoverOverride component.
            foreach (var (
                         enemyAttackHQ,
                         targetPositionPathQueued,
                         targetPositionPathQueuedEnabled,
                         target) 
                     in 
                     SystemAPI.Query<
                         RefRO<EnemyAttackHQ>,
                         RefRW<TargetPositionPathQueued>,
                         EnabledRefRW<TargetPositionPathQueued>,
                         RefRO<Target>
                     >().WithDisabled<UnitMoverOverride>().WithPresent<TargetPositionPathQueued>())
            {
                // If the unit already has a target, skip to the next entity.
                if (target.ValueRO.targetEntity != Entity.Null)
                {
                    continue;
                }
            
                // Set the destination of the unit to the position of the friendly HQ.
                targetPositionPathQueued.ValueRW.targetPosition = hqPosition;
                targetPositionPathQueuedEnabled.ValueRW = true;
            }
        }
    }
}
