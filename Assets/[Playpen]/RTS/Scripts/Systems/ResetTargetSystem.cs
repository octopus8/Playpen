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
        /// <summary>Lookup for LocalTransform components, allowing read-only access.</summary>
        private ComponentLookup<LocalTransform> _localTransformComponentLookup;
        
        /// <summary>Lookup for EntityStorageInfo, allowing read-only access.</summary>
        private EntityStorageInfoLookup _entityStorageInfoLookup;
        
        
        /// <summary>
        /// OnCreate is called when the system is created.
        /// It initializes component lookups for LocalTransform and EntityStorageInfo components.
        /// </summary>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _localTransformComponentLookup =  state.GetComponentLookup<LocalTransform>(true);  
            _entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
        }
        
        
        /// <summary>
        /// OnUpdate is called every frame the system is enabled.
        /// It checks all entities with Target and TargetOverride components to see if their target entities are still valid.
        /// If a target entity has been destroyed, the target is reset to Entity.Null.
        /// </summary>
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
    
    
    /// <summary>
    /// Job that resets the target entity in the Target component if the target entity has been destroyed.
    /// </summary>
    [BurstCompile]
    public partial struct ResetTargetJob : IJobEntity
    {
        /// <summary>Lookup for LocalTransform components, allowing read-only access.</summary>
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        
        /// <summary>Lookup for EntityStorageInfo, allowing read-only access to check if an entity exists.</summary>
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;
        
        
        /// <summary>
        /// Executes the job for each entity with a Target component.
        /// If the target entity has been destroyed, resets the target to Entity.Null.
        /// </summary>
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
    
    
    /// <summary>
    /// Job that resets the target entity in the TargetOverride component if the target entity has been destroyed.
    /// </summary>
    [BurstCompile]
    public partial struct ResetTargetOverrideJob : IJobEntity
    {
        /// <summary>Lookup for LocalTransform components, allowing read-only access.</summary>
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        
        /// <summary>Lookup for EntityStorageInfo, allowing read-only access to check if an entity exists.</summary>
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;
     
        
        /// <summary>
        /// Executes the job for each entity with a TargetOverride component.
        /// If the target entity has been destroyed, resets the target to Entity.Null.
        /// </summary>
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
