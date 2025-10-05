using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace RTS
{
    
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
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
                unitMover.ValueRW.destination = localTransform.ValueRO.Position;
                ecb.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
            }
        }
    }
}


