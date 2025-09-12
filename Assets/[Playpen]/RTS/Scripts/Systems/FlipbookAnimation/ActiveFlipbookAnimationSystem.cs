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
        public float deltaTime;
        public FlipbookAnimationDataHolder flipbookAnimationDataHolder;
        
        public void Execute(ref ActiveFlipbookAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
        { 
            ref FlipbookAnimationData flipbookAnimationData = ref flipbookAnimationDataHolder.animationData.Value[(int)activeAnimation.activeAnimation];
            activeAnimation.frameTimer += deltaTime;
            if (activeAnimation.frameTimer >= flipbookAnimationData.frameDuration)
            {
                activeAnimation.frameTimer -= flipbookAnimationData.frameDuration;
                activeAnimation.frame = (activeAnimation.frame + 1) % flipbookAnimationData.totalFrames;

                materialMeshInfo.Mesh = flipbookAnimationData.intMeshIDBlobArray[activeAnimation.frame];

                if (activeAnimation.frame == 0 && activeAnimation.activeAnimation == FlipbookAnimationScriptableObject.AnimationType.SoldierShoot)
                {
                    activeAnimation.activeAnimation =
                        FlipbookAnimationScriptableObject.AnimationType.None;
                }
                if (activeAnimation.frame == 0 && activeAnimation.activeAnimation == FlipbookAnimationScriptableObject.AnimationType.ZombieMeleeAttack)
                {
                    activeAnimation.activeAnimation =
                        FlipbookAnimationScriptableObject.AnimationType.None;
                }
            }
        }
    }
    
    
}
