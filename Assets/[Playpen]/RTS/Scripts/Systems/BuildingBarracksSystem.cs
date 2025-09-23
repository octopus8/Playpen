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
                    UnitTypeID = barracksEnqueue.ValueRO.unitTypeID
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

                if (buildingBarracks.ValueRO.unitTypeID != spawnBuffer[0].UnitTypeID)
                {
                    buildingBarracks.ValueRW.unitTypeID = spawnBuffer[0].UnitTypeID;
                    
                    UnitScriptableObject activeUnit = RTSGame.Instance.units.GetUnit(buildingBarracks.ValueRW.unitTypeID);
                    buildingBarracks.ValueRW.spawnDuration = activeUnit.spawnDuration;
                }
                
                
                buildingBarracks.ValueRW.timer += SystemAPI.Time.DeltaTime;
                if (buildingBarracks.ValueRW.timer < buildingBarracks.ValueRO.spawnDuration)
                {
                    continue;
                }
                buildingBarracks.ValueRW.timer = 0f;

                UnitScriptableObject.UnitTypeID unitTypeID = spawnBuffer[0].UnitTypeID;
                UnitScriptableObject unit = RTSGame.Instance.units.GetUnit(unitTypeID);
                spawnBuffer.RemoveAt(0);
                buildingBarracks.ValueRW.onUnitQueueChanged = true;
                

                Entity spawnedUnitEntity = state.EntityManager.Instantiate(unit.GetUnit(entityReferences));
                SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                
                SystemAPI.SetComponent(spawnedUnitEntity, new MoveOverride
                {
                    targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset,
                });
                SystemAPI.SetComponentEnabled<MoveOverride>(spawnedUnitEntity, true);
            }
            
        }
        
    }
    
}

