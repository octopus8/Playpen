using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS
{
    [UpdateAfter(typeof(ShootAttackSystem))]
    [UpdateAfter(typeof(MeleeAttackSystem))]
    partial struct FlipbookAnimationStateSystem : ISystem
    {
        private ComponentLookup<ActiveFlipbookAnimation> _activeAnimationLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlipbookAnimationDataHolder>();
            _activeAnimationLookup = state.GetComponentLookup<ActiveFlipbookAnimation>(false);
        }
        
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _activeAnimationLookup.Update(ref state);
            IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new IdleWalkingAnimationStateJob
            {
                activeAnimationLookup = _activeAnimationLookup,
            };
            idleWalkingAnimationStateJob.ScheduleParallel();
            
            _activeAnimationLookup.Update(ref state);
            AimShootAnimationStateJob aimShootAnimationStateJob = new AimShootAnimationStateJob
            {
                activeAnimationLookup = _activeAnimationLookup,
            };
            aimShootAnimationStateJob.ScheduleParallel();

            _activeAnimationLookup.Update(ref state);
            MeleeAttackAnimationStateJob meleeAttackAnimationStateJob = new MeleeAttackAnimationStateJob
            {
                activeAnimationLookup = _activeAnimationLookup,
            };
            meleeAttackAnimationStateJob.ScheduleParallel();
        }
        
        
        [BurstCompile]
        public partial struct IdleWalkingAnimationStateJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<ActiveFlipbookAnimation> activeAnimationLookup;
            
            
            public void Execute(in FlipbookAnimationMesh mesh, in UnitMover mover, in UnitFlipbookAnimations animations)
            {
                RefRW<ActiveFlipbookAnimation> activeAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                
                if (mover.isMoving)
                {
                    activeAnimation.ValueRW.nextAnimation = animations.walkAnimation;
                }
                else
                {
                    activeAnimation.ValueRW.nextAnimation = animations.idleAnimation;
                }
            }
        }
     
        
        
        [BurstCompile]
        public partial struct AimShootAnimationStateJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<ActiveFlipbookAnimation> activeAnimationLookup;
            
            public void Execute(in FlipbookAnimationMesh mesh, in ShootAttack shootAttack, in UnitMover mover, in Target target, in UnitFlipbookAnimations animations)
            {
                if (!mover.isMoving && target.targetEntity != Entity.Null)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.aimAnimation;
                }
                if (shootAttack.onShootEvent.isTriggered)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.shootAnimation;
                }
            }
        }


        [BurstCompile]
        public partial struct MeleeAttackAnimationStateJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public ComponentLookup<ActiveFlipbookAnimation> activeAnimationLookup;

            public void Execute(in FlipbookAnimationMesh mesh, in MeleeAttack meleeAttack,
                in UnitFlipbookAnimations animations)
            {
                if (meleeAttack.onAttackTarget)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.meleeAttackAnimation;
                }
            }
        }
    }
}
