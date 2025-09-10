using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RTS
{
    [UpdateAfter(typeof(ShootAttackSystem))]
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
                RefRW<ActiveFlipbookAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                
                if (mover.ValueRO.isMoving)
                {
                    activeAnimation.ValueRW.nextAnimation = animations.ValueRO.walkAnimation;
                }
                else
                {
                    activeAnimation.ValueRW.nextAnimation = animations.ValueRO.idleAnimation;
                }
            }
            
            foreach (var (
                         mesh,
                         shootAttack,
                            mover,
                         target,
                         animations) 
                     in
                     SystemAPI.Query<
                         RefRW<FlipbookAnimationMesh>,
                         RefRO<ShootAttack>,
                         RefRO<UnitMover>,
                         RefRO<Target>,
                         RefRO<UnitFlipbookAnimations>>())
            {
                if (!mover.ValueRO.isMoving && target.ValueRO.targetEntity != Entity.Null)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.aimAnimation;
                }
                if (shootAttack.ValueRO.onShootEvent.isTriggered)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.shootAnimation;
                }
            }
        }
    }
}
