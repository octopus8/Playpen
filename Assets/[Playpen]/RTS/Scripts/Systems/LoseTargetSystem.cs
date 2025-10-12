using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{
    /// <summary>
    /// System that makes entities with the LoseTarget component lose their target if they move too far away from it.
    /// </summary>
    partial struct LoseTargetSystem : ISystem
    {

        /// <summary>
        /// OnUpdate is called every frame the system is enabled.
        /// It checks if entities with a Target component should lose their target based on the LoseTarget component.
        /// If the target is too far away, the target is set to Entity.Null.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with LocalTransform, Target, LoseTarget, and TargetOverride components.
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
                // If there is no current target, skip to the next entity.
                if (target.ValueRO.targetEntity == Entity.Null)
                {
                    continue;
                }

                // If there is a target override, set the target to the override target and skip to the next entity.
                // Blee Note: This seems out of place. Why is this here in LoseTargetSystem?
                if (targetOverride.ValueRO.targetEntity != Entity.Null)
                {
                    target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                    continue;
                }
                
                // If the distance to the target is greater than the lose target distance, clear the target.
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
