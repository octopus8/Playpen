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

        
        private void OnUpdateNoJobs(ref SystemState state)
        {
            FlipbookAnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<FlipbookAnimationDataHolder>();
            foreach (var (
                         activeAnimation,
                         materialMeshInfo
                         ) in
                     SystemAPI.Query<RefRW<ActiveFlipbookAnimation>, RefRW<MaterialMeshInfo>>())
            {
                // If the current animation is a one-shot (like shooting or melee attack), do not change the animation.
                if (FlipbookAnimationScriptableObject.IsAnimationOneShot(activeAnimation.ValueRO.activeAnimation))
                {
                    continue;
                }
                
                if (activeAnimation.ValueRO.activeAnimation != activeAnimation.ValueRO.nextAnimation)
                {
                    activeAnimation.ValueRW.frame = 0;
                    activeAnimation.ValueRW.frameTimer = 0f;
                    activeAnimation.ValueRW.activeAnimation = activeAnimation.ValueRO.nextAnimation;
                    ref FlipbookAnimationData flipbookAnimationData =
                        ref animationDataHolder.animationData.Value[(int)activeAnimation.ValueRO.activeAnimation];
                    materialMeshInfo.ValueRW.Mesh = flipbookAnimationData.intMeshIDBlobArray[0];
                }
            }
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
