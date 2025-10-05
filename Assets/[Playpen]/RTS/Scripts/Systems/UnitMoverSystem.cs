using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// System that moves units towards their destination.
    /// This system uses a job to perform the movement calculations in parallel for better performance.
    /// </summary>
    partial struct UnitMoverSystem : ISystem
    {
        // Squared distance threshold to consider that the unit has reached its destination.
        public const float REACHED_TARGET_POSITION_DISTANCE_SQUARED = 2f;
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            UnitMoverJob unitMoverJob = new UnitMoverJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            };
            unitMoverJob.ScheduleParallel();
        }
    }


    /// <summary>
    /// Moves units towards their destination.
    /// This job is responsible for updating the position and rotation of units based on their movement speed and rotation speed.
    /// It checks if the unit is already at the destination and stops moving if it is.
    /// If the unit is not at the destination, it calculates the direction to the target, updates
    /// the rotation towards that direction, and sets the linear velocity to move towards the target.
    /// </summary>
    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        /// <summary> Struct parameter, time delta since last update; used to scale movement and rotation speeds. </summary>
        public float deltaTime;


        public void Execute(ref LocalTransform localTransform, ref UnitMover unitMover,
            ref PhysicsVelocity physicsVelocity)
        {
            // If the unit is already at the destination, stop moving.
            // This is done by checking if the squared distance to the destination is less than a small threshold.
            // If it is, we set the linear and angular velocities to zero.
            // This prevents the unit from overshooting the destination due to physics simulation.
            float3 moveDirection = unitMover.destination - localTransform.Position;
            float distanceToTargetSquared = math.lengthsq(moveDirection);
            if (distanceToTargetSquared <= UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQUARED)
            {
                physicsVelocity.Linear = float3.zero;
                physicsVelocity.Angular = float3.zero;
                unitMover.isMoving = false;
                return;
            }
            
            // The unit is not at the destination, so we need to move it.
            // Set the isMoving flag to true to indicate that the unit is currently moving.
            unitMover.isMoving = true;

            // Normalize the move direction to get a unit vector.
            moveDirection = math.normalize(moveDirection);
            
            // Slerp the unit's rotation towards the move direction.
            localTransform.Rotation = math.slerp(localTransform.Rotation,
                quaternion.LookRotation(moveDirection, math.up()),
                deltaTime * unitMover.rotationSpeed);
            
            // Set the linear velocity to move towards the destination.
            physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
            
            // Set the angular velocity to zero to prevent unwanted rotation.
            physicsVelocity.Angular = float3.zero;
        }
    }
}
