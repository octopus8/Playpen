using RTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace RTS
{
    [UpdateBefore(typeof(ActiveFlipbookAnimationSystem))]
    partial struct ChangeFlipbookAnimationSystem : ISystem
    {
        /// <summary>
        /// OnCreate is called when the system is created. It requires the FlipbookAnimationDataHolder, ActiveFlipbookAnimation, and MaterialMeshInfo components to be present for the system to update.
        /// </summary>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlipbookAnimationDataHolder>();
            state.RequireForUpdate<ActiveFlipbookAnimation>();
            state.RequireForUpdate<MaterialMeshInfo>();
        }

        
        /// <summary>
        /// Updates the active flipbook animations for all entities.
        /// </summary>
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
    
    
    /// <summary>
    /// Job that changes the active flipbook animation if the next animation is different from the current one.
    /// If the animation is a one-shot (like shooting or melee attack), it will not be changed until it is finished.
    /// </summary>
    [BurstCompile]
    public partial struct ChangeFlipbookAnimationJob : IJobEntity
    {
        /// <summary>Holds the flipbook animation data for all animation types.</summary>
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
            
            // If the current animation is ZombieMeleeAttack, do not change the animation.
            if (activeAnimation.activeAnimation ==
                FlipbookAnimationScriptableObject.AnimationType.ZombieMeleeAttack)
            {
                return;
            }

            // If the next animation is different from the current one, change to the next animation.
            if (activeAnimation.activeAnimation != activeAnimation.nextAnimation)
            {
                activeAnimation.frame = 0;
                activeAnimation.frameTimer = 0f;
                activeAnimation.activeAnimation = activeAnimation.nextAnimation;
                ref FlipbookAnimationData flipbookAnimationData = ref animationDataHolder.animationData.Value[(int)activeAnimation.activeAnimation];
                materialMeshInfo.Mesh = flipbookAnimationData.intMeshIDBlobArray[0];
            }
        }
    }
}
