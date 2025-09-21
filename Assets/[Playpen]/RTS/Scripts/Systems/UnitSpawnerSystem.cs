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

        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityReferences entityReferences = SystemAPI.GetSingleton<EntityReferences>();
            
            foreach (var (unitSpawner, localTransform) in
                     SystemAPI.Query<
                         RefRW<UnitSpawner>,
                         RefRO<LocalTransform>
                     >())
            {
                unitSpawner.ValueRW.timer += SystemAPI.Time.DeltaTime;
                if (unitSpawner.ValueRW.timer < unitSpawner.ValueRO.spawnInterval)
                {
                    continue;
                }
                unitSpawner.ValueRW.timer = 0f;

                Entity spawnedUnitEntity = state.EntityManager.Instantiate(entityReferences.soldierEntityPrefab);
                SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            }
            
        }
    }
    
}

