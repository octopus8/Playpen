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
                if (activeAnimation.ValueRO.activeAnimation ==
                    FlipbookAnimationScriptableObject.AnimationType.SoldierShoot)
                {
                    continue;
                }
                if (activeAnimation.ValueRO.activeAnimation ==
                    FlipbookAnimationScriptableObject.AnimationType.ZombieMeleeAttack)
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
                    materialMeshInfo.ValueRW.MeshID = flipbookAnimationData.batchMeshIDBlobArray[0];
                }
            }
        }
    }
    
    
    
    [BurstCompile]
    public partial struct ChangeFlipbookAnimationJob : IJobEntity
    {
        public FlipbookAnimationDataHolder animationDataHolder;
        
        public void Execute(ref ActiveFlipbookAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
        { 
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
            
            if (activeAnimation.activeAnimation != activeAnimation.nextAnimation)
            {
                activeAnimation.frame = 0;
                activeAnimation.frameTimer = 0f;
                activeAnimation.activeAnimation = activeAnimation.nextAnimation;
                ref FlipbookAnimationData flipbookAnimationData =
                    ref animationDataHolder.animationData.Value[(int)activeAnimation.activeAnimation];
                materialMeshInfo.MeshID = flipbookAnimationData.batchMeshIDBlobArray[0];
            }
        }
    }
}
