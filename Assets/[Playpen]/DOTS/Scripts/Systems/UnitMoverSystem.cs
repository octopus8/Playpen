using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


/// <summary>
/// DOTS system to move units towards their target position.
/// This system updates the position and rotation of units based on their movement speed and rotation speed.
/// It checks if the unit is already at the target position and stops moving if it is.
/// If the unit is not at the target position, it calculates the direction to the target,
/// updates the rotation towards that direction, and sets the linear velocity to move towards the target.
/// </summary>
partial struct UnitMoverSystem : ISystem
{
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
/// Moves units towards their target position.
/// This job is responsible for updating the position and rotation of units based on their movement speed and rotation speed.
/// It checks if the unit is already at the target position and stops moving if it is.
/// If the unit is not at the target position, it calculates the direction to the target, updates
/// the rotation towards that direction, and sets the linear velocity to move towards the target.
/// </summary>
[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    /// <summary> Minimum squared distance to the target position to consider the unit as "at the target". </summary>
    private const float minTargetDistanceSquared = 0.3f;
    
    /// <summary> Delta time for the job, used to scale movement and rotation speeds. </summary>
    public float deltaTime;
    
    
    public void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        // If the unit is already at the target position, stop moving.
        // This is done by checking if the squared distance to the target position is less than a small threshold.
        // If it is, we set the linear and angular velocities to zero.
        // This prevents the unit from overshooting the target position due to physics simulation.
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;
        if (math.lengthsq(moveDirection) < minTargetDistanceSquared)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        
        // Slerp the rotation towards the target position and set the linear velocity to move towards it.
        moveDirection = math.normalize(moveDirection);
        localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, math.up()),
            deltaTime * unitMover.rotationSpeed);
        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}