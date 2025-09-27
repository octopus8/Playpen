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

        
        public void OnUpdate(ref SystemState state)
        {
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();


            foreach (var (buildingBarracks, spawnBuffer, barracksEnqueue, barracksEnqueueEnabled) in
                     SystemAPI
                         .Query<RefRW<BuildingBarracks>, DynamicBuffer<SpawnBuffer>, RefRO<BuildingBarracksUnitEnqueue>,
                             EnabledRefRW<BuildingBarracksUnitEnqueue>>())
            {
                spawnBuffer.Add(new SpawnBuffer
                {
                    UnitType = barracksEnqueue.ValueRO.UnitType
                });
                barracksEnqueueEnabled.ValueRW = false;
                buildingBarracks.ValueRW.onUnitQueueChanged = true;
            }
            
            foreach (var (buildingBarracks, localTransform, spawnBuffer) in
                     SystemAPI.Query<
                         RefRW<BuildingBarracks>,
                         RefRO<LocalTransform>,
                         DynamicBuffer<SpawnBuffer>
                     >())
            {
                if (spawnBuffer.IsEmpty)
                {
                    continue;
                }

                if (buildingBarracks.ValueRO.UnitType != spawnBuffer[0].UnitType)
                {
                    buildingBarracks.ValueRW.UnitType = spawnBuffer[0].UnitType;
                    
                    UnitScriptableObject activeUnit = RTSGame.Instance.units.GetUnit(buildingBarracks.ValueRW.UnitType);
                    buildingBarracks.ValueRW.spawnDuration = activeUnit.spawnDuration;
                }
                
                
                buildingBarracks.ValueRW.timer += SystemAPI.Time.DeltaTime;
                if (buildingBarracks.ValueRW.timer < buildingBarracks.ValueRO.spawnDuration)
                {
                    continue;
                }
                buildingBarracks.ValueRW.timer = 0f;

                UnitScriptableObject.UnitType unitType = spawnBuffer[0].UnitType;
                UnitScriptableObject unit = RTSGame.Instance.units.GetUnit(unitType);
                spawnBuffer.RemoveAt(0);
                buildingBarracks.ValueRW.onUnitQueueChanged = true;
                

                Entity spawnedUnitEntity = state.EntityManager.Instantiate(unit.GetUnit(entityReferences));
                SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                
                SystemAPI.SetComponent(spawnedUnitEntity, new UnitMoverOverride
                {
                    targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset,
                });
                SystemAPI.SetComponentEnabled<UnitMoverOverride>(spawnedUnitEntity, true);
            }
            
        }
        
    }
    
}

