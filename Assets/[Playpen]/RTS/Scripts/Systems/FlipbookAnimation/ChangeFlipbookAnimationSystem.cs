using RTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace RTS
{
    [UpdateBefore(typeof(ActiveFlipbookAnimationSystem))]
    partial struct ChangeFlipbookAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlipbookAnimationDataHolder>();
            state.RequireForUpdate<ActiveFlipbookAnimation>();
            state.RequireForUpdate<MaterialMeshInfo>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ChangeFlipbookAnimationJob job = new ChangeFlipbookAnimationJob()
            {
                animationDataHolder = SystemAPI.GetSingleton<FlipbookAnimationDataHolder>()
            };
            job.ScheduleParallel();
        }

    }
    
    
    
    [BurstCompile]
    public partial struct ChangeFlipbookAnimationJob : IJobEntity
    {
        public FlipbookAnimationDataHolder animationDataHolder;
        
        /// <summary>
        /// Changes the active animation if the next animation is different from the current one.
        /// If the animation is a one-shot (like shooting or melee attack), it will not be changed until it is finished.
        /// </summary>
        public void Execute(ref ActiveFlipbookAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
        { 
            // If the current animation is SoldierShoot or ZombieMeleeAttack, do not change the animation.
            if (activeAnimation.activeAnimation ==
                FlipbookAnimationScriptableObject.AnimationType.SoldierShoot)
            {
                return;
            }
            if (activeAnimation.activeAnimation ==
                FlipbookAnimationScriptableObject.AnimationType.ZombieMeleeAttack)
            {
                return;
            }

            // Change the animation if the next animation is different from the current one.
            if (activeAnimation.activeAnimation != activeAnimation.nextAnimation)
            {
                activeAnimation.frame = 0;
                activeAnimation.frameTimer = 0f;
                activeAnimation.activeAnimation = activeAnimation.nextAnimation;
                ref FlipbookAnimationData flipbookAnimationData =
                    ref animationDataHolder.animationData.Value[(int)activeAnimation.activeAnimation];
                materialMeshInfo.Mesh = flipbookAnimationData.intMeshIDBlobArray[0];
            }
        }
    }
}
