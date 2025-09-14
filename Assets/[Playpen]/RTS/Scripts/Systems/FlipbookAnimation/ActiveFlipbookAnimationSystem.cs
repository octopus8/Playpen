using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace RTS
{
    
    partial struct ActiveFlipbookAnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlipbookAnimationDataHolder>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ActiveFlipbookAnimationJob job = new ActiveFlipbookAnimationJob()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                flipbookAnimationDataHolder = SystemAPI.GetSingleton<FlipbookAnimationDataHolder>()
            };
            job.ScheduleParallel();
        }


        private void OnUpdateNoJobs(ref SystemState state)
        {
            FlipbookAnimationDataHolder flipbookAnimationDataHolder = SystemAPI.GetSingleton<FlipbookAnimationDataHolder>();
            foreach (var (
                         activeAnimation, 
                         materialMeshInfo
                         ) in
                     SystemAPI.Query<
                         RefRW<ActiveFlipbookAnimation>,
                         RefRW<MaterialMeshInfo>
                     >())
            {
                ref FlipbookAnimationData flipbookAnimationData = ref flipbookAnimationDataHolder.animationData.Value[(int)activeAnimation.ValueRW.activeAnimation];
                activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
                if (activeAnimation.ValueRW.frameTimer >= flipbookAnimationData.frameDuration)
                {
                    activeAnimation.ValueRW.frameTimer -= flipbookAnimationData.frameDuration;
                    activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % flipbookAnimationData.totalFrames;

                    materialMeshInfo.ValueRW.Mesh = flipbookAnimationData.intMeshIDBlobArray[activeAnimation.ValueRW.frame];

                    if (activeAnimation.ValueRW.frame == 0 && activeAnimation.ValueRO.activeAnimation == FlipbookAnimationScriptableObject.AnimationType.SoldierShoot)
                    {
                        activeAnimation.ValueRW.activeAnimation =
                            FlipbookAnimationScriptableObject.AnimationType.None;
                    }
                    if (activeAnimation.ValueRW.frame == 0 && activeAnimation.ValueRO.activeAnimation == FlipbookAnimationScriptableObject.AnimationType.ZombieMeleeAttack)
                    {
                        activeAnimation.ValueRW.activeAnimation =
                            FlipbookAnimationScriptableObject.AnimationType.None;
                    }
                }
            }
        }
    }
    
    
    
    
    [BurstCompile]
    public partial struct ActiveFlipbookAnimationJob : IJobEntity
    {
        /// <summary>The time elapsed since the last frame.</summary>
        public float deltaTime;
        
        /// <summary>Holds the flipbook animation data for all animation types.</summary>
        public FlipbookAnimationDataHolder flipbookAnimationDataHolder;

        
        /// <summary>
        /// Sets the mesh of the entity based on its current active animation and frame.
        /// Advances the frame based on the frame duration and deltaTime.
        /// If the animation is a one-shot (like shooting or melee attack) and has completed, resets to None.
        /// </summary>
        public void Execute(ref ActiveFlipbookAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
        { 
            // Get the animation data for the current active animation.
            ref FlipbookAnimationData flipbookAnimationData = ref flipbookAnimationDataHolder.animationData.Value[(int)activeAnimation.activeAnimation];
            
            // Update the frame timer.
            activeAnimation.frameTimer += deltaTime;
            
            // If the frame timer exceeds the frame duration, advance to the next frame.
            if (activeAnimation.frameTimer >= flipbookAnimationData.frameDuration)
            {
                // Subtract the frame duration from the timer.
                activeAnimation.frameTimer -= flipbookAnimationData.frameDuration;
                // Advance to the next frame, wrapping around if necessary.
                activeAnimation.frame = (activeAnimation.frame + 1) % flipbookAnimationData.totalFrames;
                // Update the mesh to the current frame's mesh.
                materialMeshInfo.Mesh = flipbookAnimationData.intMeshIDBlobArray[activeAnimation.frame];

                // If the animation is a one-shot (like shooting or melee attack) and has completed, reset to None.
                if (activeAnimation.frame == 0 && FlipbookAnimationScriptableObject.IsAnimationOneShot(activeAnimation.activeAnimation))
                {
                    activeAnimation.activeAnimation =
                        FlipbookAnimationScriptableObject.AnimationType.None;
                }
            }
        }
    }
    
    
}
