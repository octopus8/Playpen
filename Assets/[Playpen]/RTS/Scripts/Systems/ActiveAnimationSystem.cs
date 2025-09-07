using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace RTS
{
    partial struct ActiveAnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AnimationDataHolder>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
            foreach (var (
                         activeAnimation, 
                         materialMeshInfo
                         ) in
                     SystemAPI.Query<
                         RefRW<ActiveAnimation>,
                         RefRW<MaterialMeshInfo>
                     >())
            {
                ref AnimationData animationData = ref animationDataHolder.animationData.Value[activeAnimation.ValueRW.activeAnimationIndex];
                activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
                if (activeAnimation.ValueRW.frameTimer >= animationData.frameDuration)
                {
                    activeAnimation.ValueRW.frameTimer -= animationData.frameDuration;
                    activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % animationData.totalFrames;

                    materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIDBlobArray[activeAnimation.ValueRW.frame];
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        
        }
    }
}
