using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{
    partial struct LoseTargetSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                             localTransform,
                             target,
                            loseTarget,
                             targetOverride
                         )
                     in
                     SystemAPI.Query<
                             RefRO<LocalTransform>,
                             RefRW<Target>,
                            RefRO<LoseTarget>,
                             RefRW<TargetOverride>
                     >())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                {
                    continue;
                }

                if (targetOverride.ValueRO.targetEntity != Entity.Null)
                {
                    target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                    continue;
                }
                

                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                float distanceToTarget = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
                if (distanceToTarget > loseTarget.ValueRO.lostDistance)
                {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
        }

    }
}
