using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS
{
    [UpdateAfter(typeof(ShootAttackSystem))]
    [UpdateAfter(typeof(MeleeAttackSystem))]
    partial struct FlipbookAnimationStateSystem : ISystem
    {
        /// <summary>Lookup for ActiveFlipbookAnimation components.</summary>
        private ComponentLookup<ActiveFlipbookAnimation> _activeAnimationLookup;
        
        /// <summary>
        /// OnCreate is called when the system is created. It requires the FlipbookAnimationDataHolder singleton to be present for the system to update.
        /// A reference to the ActiveFlipbookAnimation component lookup is also obtained for use in jobs.
        /// </summary>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlipbookAnimationDataHolder>();
            _activeAnimationLookup = state.GetComponentLookup<ActiveFlipbookAnimation>(false);
        }
        
        
        /// <summary>
        /// Updates the animation states for all entities based on their movement and attack states.
        /// It schedules jobs to handle idle/walking, aiming/shooting, and melee attack animations.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Schedule the IdleWalkingAnimationStateJob to update idle and walking animations.
            _activeAnimationLookup.Update(ref state);
            IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new IdleWalkingAnimationStateJob
            {
                activeAnimationLookup = _activeAnimationLookup,
            };
            idleWalkingAnimationStateJob.ScheduleParallel();
            
            // Schedule the AimShootAnimationStateJob to update aiming and shooting animations.
            _activeAnimationLookup.Update(ref state);
            AimShootAnimationStateJob aimShootAnimationStateJob = new AimShootAnimationStateJob
            {
                activeAnimationLookup = _activeAnimationLookup,
            };
            aimShootAnimationStateJob.ScheduleParallel();

            // Schedule the MeleeAttackAnimationStateJob to update melee attack animations.
            _activeAnimationLookup.Update(ref state);
            MeleeAttackAnimationStateJob meleeAttackAnimationStateJob = new MeleeAttackAnimationStateJob
            {
                activeAnimationLookup = _activeAnimationLookup,
            };
            meleeAttackAnimationStateJob.ScheduleParallel();
        }
        

        /// <summary>
        /// Job that updates the animation state based on whether the unit is moving or idle.
        /// If the unit is moving, sets the next animation to walk; otherwise, sets it to idle.
        /// </summary>
        [BurstCompile]
        public partial struct IdleWalkingAnimationStateJob : IJobEntity
        {
            // Lookup for ActiveFlipbookAnimation components, allowing read and write access.
            [NativeDisableParallelForRestriction] public ComponentLookup<ActiveFlipbookAnimation> activeAnimationLookup;
            
            
            /// <summary>
            /// Updates the animation state based on whether the unit is moving or idle.
            /// If the unit is moving, sets the next animation to walk; otherwise, sets it to idle.
            /// </summary>
            public void Execute(in FlipbookAnimationMesh mesh, in UnitMover mover, in UnitFlipbookAnimations animations)
            {
                // Get a reference to the ActiveFlipbookAnimation component for the entity's mesh.
                RefRW<ActiveFlipbookAnimation> activeAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                
                // If the unit is moving, set the next animation to walk; otherwise, set it to idle.
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
     
        
        /// <summary>
        /// Job that updates the animation state based on aiming and shooting actions.
        /// If the unit is not moving and has a target, sets the next animation to aim.
        /// If the shoot attack event is triggered, sets the next animation to shoot.
        /// </summary>
        [BurstCompile]
        public partial struct AimShootAnimationStateJob : IJobEntity
        {
            // Lookup for ActiveFlipbookAnimation components, allowing read and write access.
            [NativeDisableParallelForRestriction] public ComponentLookup<ActiveFlipbookAnimation> activeAnimationLookup;
            
            
            /// <summary>
            /// Updates the animation state based on aiming and shooting actions.
            /// If the unit is not moving and has a target, sets the next animation to aim.
            /// If the shoot attack event is triggered, sets the next animation to shoot.
            /// </summary>
            public void Execute(in FlipbookAnimationMesh mesh, in ShootAttack shootAttack, in UnitMover mover, in Target target, in UnitFlipbookAnimations animations)
            {
                // If the unit is not moving and has a target, set the next animation to aim.
                if (!mover.isMoving && target.targetEntity != Entity.Null)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.aimAnimation;
                }
                
                // If the shoot attack event is triggered, set the next animation to shoot.
                if (shootAttack.onShootEvent.isTriggered)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.shootAnimation;
                }
            }
        }


        /// <summary>
        /// Job that updates the animation state based on melee attack actions.
        /// If the melee attack event is triggered, sets the next animation to melee attack.
        /// </summary>
        [BurstCompile]
        public partial struct MeleeAttackAnimationStateJob : IJobEntity
        {
            // Lookup for ActiveFlipbookAnimation components, allowing read and write access.
            [NativeDisableParallelForRestriction] public ComponentLookup<ActiveFlipbookAnimation> activeAnimationLookup;

            
            /// <summary>
            /// Updates the animation state based on melee attack actions.
            /// If the melee attack event is triggered, sets the next animation to melee attack.
            /// </summary>
            public void Execute(in FlipbookAnimationMesh mesh, in MeleeAttack meleeAttack, in UnitFlipbookAnimations animations)
            {
                // If the melee attack event is triggered, set the next animation to melee attack.
                if (meleeAttack.onAttackTarget)
                {
                    RefRW<ActiveFlipbookAnimation> activeFlipbookAnimation = activeAnimationLookup.GetRefRW(mesh.mesh);
                    activeFlipbookAnimation.ValueRW.nextAnimation = animations.meleeAttackAnimation;
                }
            }
        }
    }
}
