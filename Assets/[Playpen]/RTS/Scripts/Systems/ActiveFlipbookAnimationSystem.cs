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
            FlipbookAnimationDataHolder flipbookAnimationDataHolder = SystemAPI.GetSingleton<FlipbookAnimationDataHolder>();
            foreach (var (
                         activeAnimation, 
                         materialMeshInfo
                         ) in
                     SystemAPI.Query<
                         RefRW<ActiveAnimation>,
                         RefRW<MaterialMeshInfo>
                     >())
            {
                // For test, if no animation is set, set to idle
                if (activeAnimation.ValueRW.activeAnimation == FlipbookAnimationScriptableObject.AnimationType.SoldierNone)
                {
                    activeAnimation.ValueRW.activeAnimation =
                        FlipbookAnimationScriptableObject.AnimationType.SoldierWalk;
                }
                
                ref FlipbookAnimationData flipbookAnimationData = ref flipbookAnimationDataHolder.animationData.Value[(int)activeAnimation.ValueRW.activeAnimation];
                activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
                if (activeAnimation.ValueRW.frameTimer >= flipbookAnimationData.frameDuration)
                {
                    activeAnimation.ValueRW.frameTimer -= flipbookAnimationData.frameDuration;
                    activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % flipbookAnimationData.totalFrames;

                    materialMeshInfo.ValueRW.MeshID = flipbookAnimationData.batchMeshIDBlobArray[activeAnimation.ValueRW.frame];
                }
            }
        }
    }
}
