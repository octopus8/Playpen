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
                         mover) 
                     in
                     SystemAPI.Query<RefRW<FlipbookAnimationMesh>, RefRO<UnitMover>>())
            {
                RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                
                FlipbookAnimationScriptableObject.AnimationType prevAnimationType = activeFlipbookAnimation.ValueRO.activeAnimation;
                
                if (mover.ValueRO.isMoving)
                {
                    activeFlipbookAnimation.ValueRW.nextAnimation =
                        FlipbookAnimationScriptableObject.AnimationType.SoldierWalk;
                }
                else
                {
                    activeFlipbookAnimation.ValueRW.nextAnimation = FlipbookAnimationScriptableObject.AnimationType.SoldierIdle;
                }
            }
        }
    }
}
