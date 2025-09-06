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
        
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                         activeAnimation, 
                         materialMeshInfo
                         ) in
                     SystemAPI.Query<
                         RefRW<ActiveAnimation>,
                         RefRW<MaterialMeshInfo>
                     >())
            {
                activeAnimation.ValueRW.frameTime += SystemAPI.Time.DeltaTime;
                if (activeAnimation.ValueRW.frameTime >= activeAnimation.ValueRW.frameDuration)
                {
                    activeAnimation.ValueRW.frameTime -= activeAnimation.ValueRW.frameDuration;
                    activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % activeAnimation.ValueRW.totalFrames;

                    switch (activeAnimation.ValueRW.frame)
                    {
                        case 0:
                            materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame0;
                            break;
                        case 1:
                            materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame1;
                            break;
                    }
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        
        }
    }
}
