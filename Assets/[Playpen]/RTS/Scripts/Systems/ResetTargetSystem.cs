using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


namespace RTS
{
    /// <summary>
    /// System that resets the target entity in the Target component if the target entity has been destroyed.
    /// This system runs at the beginning of the SimulationSystemGroup to ensure targets are valid before other systems process them.
    /// </summary> 
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct ResetTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformComponentLookup;
        private EntityStorageInfoLookup _entityStorageInfoLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _localTransformComponentLookup =  state.GetComponentLookup<LocalTransform>(true);  
            _entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
        }
        
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _localTransformComponentLookup.Update(ref state);
            _entityStorageInfoLookup.Update(ref state);
            ResetTargetJob resetTargetJob = new ResetTargetJob
            {
                localTransformComponentLookup = _localTransformComponentLookup,
                entityStorageInfoLookup = _entityStorageInfoLookup
            };
            resetTargetJob.ScheduleParallel();
            
            ResetTargetOverrideJob resetTargetOverrideJob = new ResetTargetOverrideJob
            {
                localTransformComponentLookup = _localTransformComponentLookup,
                entityStorageInfoLookup = _entityStorageInfoLookup
            };
            resetTargetOverrideJob.ScheduleParallel();
        }

    }
    
    
    [BurstCompile]
    public partial struct ResetTargetJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;
        
        public void Execute(ref Target target)
        {
            // If no target, skip.
            if (target.targetEntity == Entity.Null)
            {
                return;
            }

            // If target entity has been destroyed, reset target to null.
            // Note: We also check if the entity has a LocalTransform component to detect
            // entities that might not have been fully destroyed (complete destruction seems to occur in "Parent System" due to "cleanup components").
            if (!entityStorageInfoLookup.Exists(target.targetEntity) ||
                !localTransformComponentLookup.HasComponent(target.targetEntity))
            {
                target.targetEntity = Entity.Null;
            }
        }
    }
    
    
    [BurstCompile]
    public partial struct ResetTargetOverrideJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;
        
        public void Execute(ref TargetOverride targetOverride)
        {
            // If no target, skip.
            if (targetOverride.targetEntity == Entity.Null)
            {
                return;
            }

            // If target entity has been destroyed, reset target to null.
            // Note: We also check if the entity has a LocalTransform component to detect
            // entities that might not have been fully destroyed (complete destruction seems to occur in "Parent System" due to "cleanup components").
            if (!entityStorageInfoLookup.Exists(targetOverride.targetEntity) ||
                !localTransformComponentLookup.HasComponent(targetOverride.targetEntity))
            {
                targetOverride.targetEntity = Entity.Null;
            }
        }
    }
    
    
}
