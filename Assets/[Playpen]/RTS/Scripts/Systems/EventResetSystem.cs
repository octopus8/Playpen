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
        /// <summary>Array to hold job handles for the reset jobs.</summary>
        private NativeArray<JobHandle> jobHandles;
        
        
        /// <summary>
        /// OnCreate is called when the system is created. It initializes a NativeArray to hold job handles for the reset jobs.
        /// </summary>
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
 
    
    /// <summary>
    /// Job that resets shoot attack-related event flags after they have been processed.
    /// It sets the onShootEvent.isTriggered flag to false.
    /// </summary>
    [BurstCompile]
    public partial struct ResetShootAttackEventsJob : IJobEntity
    {
        /// <summary>
        /// Job that resets the onShootEvent.isTriggered flag in the ShootAttack component after it has been processed.
        /// It sets the isTriggered flag to false.
        /// </summary>
        public void Execute(ref ShootAttack shootAttack)
        {
            shootAttack.onShootEvent.isTriggered = false;
        }
    }
    
    
    /// <summary>
    /// Job that resets health-related event flags after they have been processed.
    /// It sets onHealthChanged and onDead flags to false.
    /// </summary>
    [BurstCompile]
    public partial struct ResetHealthEventsJob : IJobEntity
    {
        /// <summary>
        /// Resets the onHealthChanged and onDead flags in the Health component after they have been processed.
        /// It sets both flags to false.
        /// </summary>
        public void Execute(ref Health health)
        {
            health.onHealthChanged = false;
            health.onDead = false;
        }
    }
    

    /// <summary>
    /// Job that resets selection-related event flags after they have been processed.
    /// It sets onSelected and onDeselected flags to false.
    /// </summary>
    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct ResetSelectedEventsJob : IJobEntity
    {
        /// <summary>
        /// Resets the onSelected and onDeselected flags in the UnitSelected component after they have been processed.
        /// It sets both flags to false.
        /// </summary>
        public void Execute(ref UnitSelected unitSelected)
        {
            unitSelected.onSelected = false;
            unitSelected.onDeselected = false;
        }
    }
    
    
    /// <summary>
    /// Job that resets melee attack-related event flags after they have been processed.
    /// It sets the onAttackTarget flag to false.
    /// </summary>
    [BurstCompile]
    public partial struct ResetMeleeAttackEventsJob : IJobEntity
    {
        /// <summary>
        /// Resets the onAttackTarget flag in the MeleeAttack component after it has been processed.
        /// It sets the onAttackTarget flag to false.
        /// </summary>
        public void Execute(ref MeleeAttack meleeAttack)
        {
            meleeAttack.onAttackTarget = false;
        }
    }

    
    /// <summary>
    /// Job that resets BuildingBarracks-related event flags after they have been processed.
    /// It collects entities with onUnitQueueChangedEventFlag set to true and resets the flag.
    /// </summary>
    [BurstCompile]
    public partial struct ResetBuildingBarracksEventsJob : IJobEntity
    {
        /// <summary>Parallel writer for a list of entities that had their unit queue changed.</summary>
        public NativeList<Entity>.ParallelWriter onUnitQueueChangedEntities;

        
        /// <summary>
        /// Resets the onUnitQueueChangedEventFlag in the BuildingBarracks component after it has been processed.
        /// If the flag is true, the entity is added to the onUnitQueueChangedEntities list.
        /// The flag is then set to false.
        /// </summary>
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
