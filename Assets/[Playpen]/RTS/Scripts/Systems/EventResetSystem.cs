using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace RTS
{
    /// <summary>
    /// System that resets event flags at the end of each frame.
    /// This system runs in the LateSimulationSystemGroup and set OrderLast to true to ensure all events are processed before resetting.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    partial struct EventResetSystem : ISystem
    {
        private NativeArray<JobHandle> jobHandles;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            jobHandles = new NativeArray<JobHandle>(4, Allocator.Persistent);
        }
        
        
        /// <summary>
        /// Triggers events and resets event flags for various components at the end of each frame.
        /// </summary>
        public void OnUpdate(ref SystemState state)
        {
            // If the friendly HQ exists and is dead, trigger the OnHQDead event.
            if (SystemAPI.HasSingleton<BuildingFriendlyHQ>())
            {
                Health health = SystemAPI.GetComponent<Health>(SystemAPI.GetSingletonEntity<BuildingFriendlyHQ>());
                if (health.onDead)
                {
                    DOTSEvents.Instance.TriggerOnHQDead();
                }
            }
            
            // Schedule reset jobs for various event flags.
            jobHandles[0] = new ResetSelectedEventsJob().ScheduleParallel(state.Dependency);
            jobHandles[1] = new ResetHealthEventsJob().ScheduleParallel(state.Dependency);
            jobHandles[2] = new ResetShootAttackEventsJob().ScheduleParallel(state.Dependency);
            jobHandles[3] = new ResetMeleeAttackEventsJob().ScheduleParallel(state.Dependency);

            // Handle BuildingBarracks events separately to collect entities with onUnitQueueChangedEventFlag set.
            NativeList<Entity> onUnitQueueChangedEntities = new NativeList<Entity>(Allocator.TempJob);
            new ResetBuildingBarracksEventsJob
            {
                onUnitQueueChangedEntities = onUnitQueueChangedEntities.AsParallelWriter()
            }.ScheduleParallel(state.Dependency).Complete();
            
            // Trigger the OnBarracksQueueChanged event for all affected entities.
            DOTSEvents.Instance.TriggerOnBarracksQueueChanged(onUnitQueueChangedEntities);
            
            // Combine all job handles to ensure all jobs complete before the next frame.
            state.Dependency = JobHandle.CombineDependencies(jobHandles);

            // Dispose of the temporary list.
            onUnitQueueChangedEntities.Dispose();
        }
    }
 
    
    [BurstCompile]
    public partial struct ResetShootAttackEventsJob : IJobEntity
    {
        public void Execute(ref ShootAttack shootAttack)
        {
            shootAttack.onShootEvent.isTriggered = false;
        }
    }
    
    
    [BurstCompile]
    public partial struct ResetHealthEventsJob : IJobEntity
    {
        public void Execute(ref Health health)
        {
            health.onHealthChanged = false;
            health.onDead = false;
        }
    }
    
    
    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct ResetSelectedEventsJob : IJobEntity
    {
        public void Execute(ref UnitSelected unitSelected)
        {
            unitSelected.onSelected = false;
            unitSelected.onDeselected = false;
        }
    }
    
    
    [BurstCompile]
    public partial struct ResetMeleeAttackEventsJob : IJobEntity
    {
        public void Execute(ref MeleeAttack meleeAttack)
        {
            meleeAttack.onAttackTarget = false;
        }
    }

    
    [BurstCompile]
    public partial struct ResetBuildingBarracksEventsJob : IJobEntity
    {
        public NativeList<Entity>.ParallelWriter onUnitQueueChangedEntities;
        
        public void Execute(ref BuildingBarracks barracks, Entity entity)
        {
            if (barracks.onUnitQueueChangedEventFlag)
            {
                onUnitQueueChangedEntities.AddNoResize(entity);
            }
            
            barracks.onUnitQueueChangedEventFlag = false;
        }
    }
  
}
