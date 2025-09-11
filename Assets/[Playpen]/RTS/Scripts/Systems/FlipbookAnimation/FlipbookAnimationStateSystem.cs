using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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

        
        
        private void OnUpdateNoJobs(ref SystemState state)
        {
            foreach (var (
                         mesh,
                         mover,
                         animations) 
                     in
                     SystemAPI.Query<RefRW<FlipbookAnimationMesh>, RefRO<UnitMover>, RefRO<UnitFlipbookAnimations>>())
            {
                RefRW<ActiveFlipbookAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                
                if (mover.ValueRO.isMoving)
                {
                    activeAnimation.ValueRW.nextAnimation = animations.ValueRO.walkAnimation;
                }
                else
                {
                    activeAnimation.ValueRW.nextAnimation = animations.ValueRO.idleAnimation;
                }
            }
            
            foreach (var (
                         mesh,
                         shootAttack,
                            mover,
                         target,
                         animations) 
                     in
                     SystemAPI.Query<
                         RefRW<FlipbookAnimationMesh>,
                         RefRO<ShootAttack>,
                         RefRO<UnitMover>,
                         RefRO<Target>,
                         RefRO<UnitFlipbookAnimations>>())
            {
                if (!mover.ValueRO.isMoving && target.ValueRO.targetEntity != Entity.Null)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.aimAnimation;
                }
                if (shootAttack.ValueRO.onShootEvent.isTriggered)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.shootAnimation;
                }
            }
            
            
            foreach (var (
                         mesh,
                         meleeAttack,
                         animations) 
                     in
                     SystemAPI.Query<
                         RefRW<FlipbookAnimationMesh>,
                         RefRO<MeleeAttack>,
                         RefRO<UnitFlipbookAnimations>>())
            {
                if (meleeAttack.ValueRO.onAttacked)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = SystemAPI.GetComponentRW<ActiveFlipbookAnimation>(mesh.ValueRO.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.ValueRO.meleeAttackAnimation;
                }
            }
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
                if (meleeAttack.onAttacked)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.meleeAttackAnimation;
                }
            }
        }
    }
}
