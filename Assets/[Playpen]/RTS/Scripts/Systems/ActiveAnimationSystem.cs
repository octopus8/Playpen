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
                // For test, we just use the soldier idle animation for all entities.
                if (activeAnimation.ValueRO.animationDataBlobAssetReference.IsCreated == false)
                {
                    activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierIdleAnimationData;
                }
                
                activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
                if (activeAnimation.ValueRW.frameTimer >= activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameDuration)
                {
                    activeAnimation.ValueRW.frameTimer -= activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameDuration;
                    activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % activeAnimation.ValueRO.animationDataBlobAssetReference.Value.totalFrames;

                    materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.animationDataBlobAssetReference.Value.batchMeshIDBlobArray[activeAnimation.ValueRW.frame];
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        
        }
    }
}
