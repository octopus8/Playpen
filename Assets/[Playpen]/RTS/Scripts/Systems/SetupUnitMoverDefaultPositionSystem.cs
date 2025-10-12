using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// System that sets the default position for UnitMover components based on the entity's current position. This
    /// is used to initialize the destination of units that start in the scene.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct SetupUnitMoverDefaultPositionSystem : ISystem
    {
        /// <summary>
        /// OnCreate is called when the system is created. It requires the EndSimulationEntityCommandBufferSystem singleton to be present for the system to update.
        /// </summary>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        
        /// <summary>
        /// OnUpdate is called every frame the system is enabled. It sets the destination of UnitMover components to the
        /// current position of the entity and removes the SetupUnitMoverDefaultPosition component.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with LocalTransform, UnitMover, and SetupUnitMoverDefaultPosition components
            // and set the destination of the UnitMover to the current position of the entity.
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


