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
                         RefRW<UnitSpawner>,
                         RefRO<LocalTransform>,
                         DynamicBuffer<SpawnBuffer>
                     >())
            {
                if (spawnBuffer.IsEmpty)
                {
                    continue;
                }
                
                unitSpawner.ValueRW.timer += SystemAPI.Time.DeltaTime;
                if (unitSpawner.ValueRW.timer < unitSpawner.ValueRO.spawnInterval)
                {
                    continue;
                }
                unitSpawner.ValueRW.timer = 0f;

                UnitScriptableObject.UnitType unitType = spawnBuffer[0].unitType;
                UnitScriptableObject unit = RTSGame.Instance.units.GetUnit(unitType);
                spawnBuffer.RemoveAt(0);
                

                Entity spawnedUnitEntity = state.EntityManager.Instantiate(unit.GetUnit(entityReferences));
                SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            }
            
        }
        
    }
    
}

