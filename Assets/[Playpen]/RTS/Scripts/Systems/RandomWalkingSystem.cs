using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{
    partial struct RandomWalkingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (randomWalking, unitMover, localTransform) in
                     SystemAPI.Query<
                         RefRW<RandomWalking>,
                         RefRW<UnitMover>,
                         RefRO<LocalTransform>
                     >())
            {
                if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) <
                    UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQUARED)
                {
                    Random random = randomWalking.ValueRO.random;
                    float3 randomDirection = new float3(random.NextFloat(-1f, 1f), 0f, random.NextFloat(-1f, 1f));
                    randomDirection = math.normalize(randomDirection);
                    
                     float randomDistance = random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);
                    randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + randomDirection * randomDistance;
                    unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
                    
                    randomWalking.ValueRW.random = random;
                }
                else
                {
                    unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
                }
            }
        }
    }
}
