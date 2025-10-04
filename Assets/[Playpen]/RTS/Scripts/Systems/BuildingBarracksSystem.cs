using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


namespace RTS
{

    partial struct BuildingBarracksSystem : ISystem
    {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntityReferences>();
        }

        
        /// <summary>
        /// Handles the spawning of units from barracks buildings.
        /// </summary>
        /// <remarks>
        /// First, iterate over all barracks that have a unit enqueue request and add the requested unit to the spawn buffer.
        /// Then, iterate over all barracks, and if there are units in the spawn buffer and the spawn timer exceeds the spawn duration,
        /// spawn the unit at the barracks position and set its mover override to the rally position.
        /// </remarks>
        /// <param name="state"></param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all barracks that have a unit enqueue request.
            // Add the requested unit to the spawn buffer, disable the enqueue request flag,
            // and set the barracks "unit queue changed" event flag to true.
            foreach (var (
                         buildingBarracks, 
                         spawnBuffer, 
                         barracksEnqueue, 
                         barracksEnqueueEnabled) 
                     in
                     SystemAPI.Query<
                         RefRW<BuildingBarracks>, 
                         DynamicBuffer<SpawnBuffer>, 
                         RefRO<BuildingBarracksUnitEnqueue>,
                         EnabledRefRW<BuildingBarracksUnitEnqueue>
                     >())
            {
                spawnBuffer.Add(new SpawnBuffer
                {
                    UnitType = barracksEnqueue.ValueRO.UnitType
                });
                barracksEnqueueEnabled.ValueRW = false;
                buildingBarracks.ValueRW.onUnitQueueChangedEventFlag = true;
            }
            
            // Iterate over all barracks. If there are units in the spawn buffer,
            // increment the spawn timer. If the timer exceeds the spawn duration,
            // spawn the unit at the barracks position and set its mover override to the rally position.
            // Remove the spawned unit from the spawn buffer and set the barracks "unit queue changed" event flag to true.
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();
            foreach (var (
                         buildingBarracks, 
                         localTransform, 
                         spawnBuffer) 
                     in
                     SystemAPI.Query<
                         RefRW<BuildingBarracks>,
                         RefRO<LocalTransform>,
                         DynamicBuffer<SpawnBuffer>
                     >())
            {
                // If there are no units in the spawn buffer, skip this barracks.
                if (spawnBuffer.IsEmpty)
                {
                    continue;
                }

                // If the unit type to spawn has changed, update the barracks unit type and spawn duration.
                if (buildingBarracks.ValueRO.UnitType != spawnBuffer[0].UnitType)
                {
                    buildingBarracks.ValueRW.UnitType = spawnBuffer[0].UnitType;
                    UnitScriptableObject activeUnit = RTSGame.Instance.units.GetUnit(buildingBarracks.ValueRW.UnitType);
                    buildingBarracks.ValueRW.spawnDuration = activeUnit.spawnDuration;
                }
                
                // Increment the spawn timer. If the timer is less than the spawn duration, skip this barracks.
                buildingBarracks.ValueRW.timer += SystemAPI.Time.DeltaTime;
                if (buildingBarracks.ValueRW.timer < buildingBarracks.ValueRO.spawnDuration)
                {
                    continue;
                }
                
                // The spawn timer has exceeded the spawn duration.
                // Reset the timer, remove the unit from the spawn buffer, and set the barracks "unit queue changed" event flag to true.
                buildingBarracks.ValueRW.timer = 0f;
                UnitScriptableObject unit = RTSGame.Instance.units.GetUnit(spawnBuffer[0].UnitType);
                spawnBuffer.RemoveAt(0);
                buildingBarracks.ValueRW.onUnitQueueChangedEventFlag = true;
                
                // Instantiate the unit entity and set its position to the barracks position.
                Entity spawnedUnitEntity = state.EntityManager.Instantiate(unit.GetUnit(entityReferences));
                SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

                // Set the unit mover override to the barracks rally position and enable the mover override component.
                SystemAPI.SetComponent(spawnedUnitEntity, new UnitMoverOverride
                {
                    targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset,
                });
                SystemAPI.SetComponentEnabled<UnitMoverOverride>(spawnedUnitEntity, true);
            }
        }
    }
}

