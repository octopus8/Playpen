using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace RTS
{
    partial struct SetupUnitMoverDefaultPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach( var (
                        localTransform, 
                        unitMover, 
                        _,
                        entity
                        ) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<UnitMover>,
                    RefRO<SetupUnitMoverDefaultPosition>
                >().WithEntityAccess()
            )
            {
                Debug.Log("Setting up default UnitMover position");
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                ecb.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
            }
        }
    }
}


