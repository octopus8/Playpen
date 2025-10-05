using RTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{

    /// <summary>
    /// System that applies temporary movement overrides to units.
    /// </summary>
    partial struct UnitMoverOverrideSystem : ISystem
    {
        /// <summary>
        /// Overwrites the target position of units with a move override if the override is enabled.
        /// If the unit is close enough to the override target position, the override is disabled.
        /// This system ensures that units can be directed to specific positions temporarily,
        /// overriding their current movement targets, (e.g., find target).
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                         localTransform,
                         moveOverride,
                         moveOverrideEnabled,
                         unitMover
                         ) in
                     SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRO<UnitMoverOverride>,
                         EnabledRefRW<UnitMoverOverride>,
                         RefRW<UnitMover>
                     >())
            {
                if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.overrideDestination) >
                    UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQUARED)
                {
                    unitMover.ValueRW.destination = moveOverride.ValueRO.overrideDestination;
                }
                else
                {
                    moveOverrideEnabled.ValueRW = false;
                }
            }
        }
    }
}