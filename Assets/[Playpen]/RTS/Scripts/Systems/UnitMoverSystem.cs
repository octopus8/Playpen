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
    /// </summary>
    partial struct UnitMoverSystem : ISystem
    {
        /// <summary> The squared distance threshold to consider that the unit has reached its target position. </summary>
        public const float REACHED_TARGET_POSITION_DISTANCE_SQUARED = 2f;
        
        
        public void OnCreate(ref SystemState state)
        {
            // This system requires the GridSystemData singleton to be present for the system to update.
            state.RequireForUpdate<GridSystem.GridSystemData>();
        }
        
        
        /// <summary>
        /// Updates the positions of all units with a UnitMover component.
        /// This method schedules the UnitMoverJob to run in parallel.
        /// </summary>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            GridSystem.GridSystemData gridSystemData = SystemAPI.GetSingleton<GridSystem.GridSystemData>();
            
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            
            foreach (var (
                         localTransform,
                         targetPositionPathQueued,
                         targetPositionPathQueuedEnabled,
                         flowFieldPathRequest,
                         flowFieldPathRequestEnabled,
                         unitMover
                         )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<TargetPositionPathQueued>, 
                         EnabledRefRW<TargetPositionPathQueued>,
                         RefRW<FlowFieldPathRequest>,
                         EnabledRefRW<FlowFieldPathRequest>,
                         RefRW<UnitMover>
                     >().WithPresent<FlowFieldPathRequest>())
            {
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = targetPositionPathQueued.ValueRO.targetPosition,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << RTSGame.PATHFINDING_WALL_LAYER,
                        GroupIndex = 0
                    }
                };

                // If there is no wall between the unit and the queued target position, then
                // set the unit mover destination to the queued target position.
                if (!collisionWorld.CastRay(raycastInput))
                {
                    unitMover.ValueRW.destination = targetPositionPathQueued.ValueRO.targetPosition;
                }
                
                // Otherwise, there is a wall between.
                // Set the flow field path request to the queued target position path target position.
                else
                {
                    flowFieldPathRequest.ValueRW.targetPosition = targetPositionPathQueued.ValueRO.targetPosition;
                    flowFieldPathRequestEnabled.ValueRW = true;
                }

                // Queued target position path has been processed.
                // Disable component.
                targetPositionPathQueuedEnabled.ValueRW = false;
            }
            
            
            foreach (var (
                         localTransform,
                         flowFieldFollower,
                         flowFieldFollowerEnabled,
                         unitMover
                         )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<FlowFieldFollower>, 
                         EnabledRefRW<FlowFieldFollower>,
                         RefRW<UnitMover>
                     >())
            {
                int2 gridPosition = GridSystem.GetGridPosition(localTransform.ValueRO.Position, gridSystemData.gridNodeSize);
                int index = GridSystem.CalculateIndex(gridPosition, gridSystemData.width);
                Entity gridNodeEntity = gridSystemData.gridMapArray[flowFieldFollower.ValueRO.gridIndex].gridEntityArray[index];

                GridSystem.GridNode gridNode = SystemAPI.GetComponent<GridSystem.GridNode>(gridNodeEntity);
                float3 movementVector = GridSystem.GetWorldMovementVector(gridNode.vector);

                if (GridSystem.IsWall(gridNode))
                {
                    movementVector = flowFieldFollower.ValueRO.lastMoveVector;
                }
                else
                {
                    flowFieldFollower.ValueRW.lastMoveVector = movementVector;
                }
                
                unitMover.ValueRW.destination = GridSystem.GetWorldCenterPosition(gridPosition.x, gridPosition.y, gridSystemData.gridNodeSize)
                                                + movementVector * (gridSystemData.gridNodeSize * 2.0f);
                
                if (math.distance(localTransform.ValueRO.Position, flowFieldFollower.ValueRO.targetPosition) < gridSystemData.gridNodeSize)
                {
                    unitMover.ValueRW.destination = localTransform.ValueRO.Position;
                    flowFieldFollowerEnabled.ValueRW = false;
                }
                
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = flowFieldFollower.ValueRO.targetPosition,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << RTSGame.PATHFINDING_WALL_LAYER,
                        GroupIndex = 0
                    }
                };

                // If there is no wall between the unit and the queued target position, then
                // set the unit mover destination to the queued target position.
                if (!collisionWorld.CastRay(raycastInput))
                {
                    unitMover.ValueRW.destination = flowFieldFollower.ValueRO.targetPosition;
                    flowFieldFollowerEnabled.ValueRW = false;
                }
            }
            
            
            
            
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


        /// <summary>
        /// Moves a unit towards its destination.
        /// </summary>
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
