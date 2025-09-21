using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


namespace RTS
{

    partial struct UnitSpawnerSystem : ISystem
    {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntityReferences>();
        }

        
        public void OnUpdate(ref SystemState state)
        {
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();
            
            foreach (var (unitSpawner, localTransform, spawnBuffer) in
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

                if (unitSpawner.ValueRO.unitTypeID != spawnBuffer[0].UnitTypeID)
                {
                    unitSpawner.ValueRW.unitTypeID = spawnBuffer[0].UnitTypeID;
                    
                    UnitScriptableObject activeUnit = RTSGame.Instance.units.GetUnit(unitSpawner.ValueRW.unitTypeID);
                    unitSpawner.ValueRW.spawnDuration = activeUnit.spawnDuration;
                }
                
                
                unitSpawner.ValueRW.timer += SystemAPI.Time.DeltaTime;
                if (unitSpawner.ValueRW.timer < unitSpawner.ValueRO.spawnDuration)
                {
                    continue;
                }
                unitSpawner.ValueRW.timer = 0f;

                UnitScriptableObject.UnitTypeID unitTypeID = spawnBuffer[0].UnitTypeID;
                UnitScriptableObject unit = RTSGame.Instance.units.GetUnit(unitTypeID);
                spawnBuffer.RemoveAt(0);
                

                Entity spawnedUnitEntity = state.EntityManager.Instantiate(unit.GetUnit(entityReferences));
                SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                
                SystemAPI.SetComponent(spawnedUnitEntity, new MoveOverride
                {
                    targetPosition = localTransform.ValueRO.Position + unitSpawner.ValueRO.rallyPositionOffset,
                });
                SystemAPI.SetComponentEnabled<MoveOverride>(spawnedUnitEntity, true);
            }
            
        }
        
    }
    
}

