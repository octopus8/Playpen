using RTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace RTS
{
    [UpdateBefore(typeof(ActiveFlipbookAnimationSystem))]
    partial struct ChangeFlipbookAnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            FlipbookAnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<FlipbookAnimationDataHolder>();
            foreach (var (
                         activeAnimation,
                         materialMeshInfo
                         ) in
                     SystemAPI.Query<RefRW<ActiveFlipbookAnimation>, RefRW<MaterialMeshInfo>>())
            {
                if (activeAnimation.ValueRO.activeAnimation != activeAnimation.ValueRO.nextAnimation)
                {
                    activeAnimation.ValueRW.frame = 0;
                    activeAnimation.ValueRW.frameTimer = 0f;
                    activeAnimation.ValueRW.activeAnimation = activeAnimation.ValueRO.nextAnimation;
                    ref FlipbookAnimationData flipbookAnimationData =
                        ref animationDataHolder.animationData.Value[(int)activeAnimation.ValueRO.activeAnimation];
                    materialMeshInfo.ValueRW.MeshID = flipbookAnimationData.batchMeshIDBlobArray[0];
                }
            }
        }
    }
}
