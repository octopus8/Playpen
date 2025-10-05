using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace RTS
{
    /// <summary>
    /// System that makes entities with RandomWalking component move randomly within a specified range.
    /// </summary>
    partial struct RandomWalkingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with RandomWalking, UnitMover, and LocalTransform components.
            foreach (var (randomWalking, unitMover, localTransform) in
                     SystemAPI.Query<
                         RefRW<RandomWalking>,
                         RefRW<UnitMover>,
                         RefRO<LocalTransform>
                     >())
            {
                // If the entity has reached its target position, select a new random target position within the specified range.
                if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) <
                    UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQUARED)
                {
                    Random random = randomWalking.ValueRO.random;
                    float3 randomDirection = new float3(random.NextFloat(-1f, 1f), 0f, random.NextFloat(-1f, 1f));
                    randomDirection = math.normalize(randomDirection);
                    
                     float randomDistance = random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);
                    randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + randomDirection * randomDistance;
                    unitMover.ValueRW.destination = randomWalking.ValueRO.targetPosition;
                    
                    randomWalking.ValueRW.random = random;
                }
                // Otherwise, continue moving towards the current target position.
                else
                {
                    unitMover.ValueRW.destination = randomWalking.ValueRO.targetPosition;
                }
            }
        }
    }
}
