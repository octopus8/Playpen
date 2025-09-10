using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RTS
{
    partial struct FlipbookAnimationStateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                         mesh,
                         mover,
                         animations) 
                     in
                     SystemAPI.Query<RefRW<FlipbookAnimationMesh>, RefRO<UnitMover>, RefRO<UnitFlipbookAnimations>>())
            {
                RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                
                if (mover.ValueRO.isMoving)
                {
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.walkAnimation;
                }
                else
                {
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.idleAnimation;
                }
            }
        }
    }
}
