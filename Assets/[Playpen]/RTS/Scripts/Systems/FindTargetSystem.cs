using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;


namespace RTS
{

    partial struct FindTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
            foreach ((
                         RefRO<LocalTransform> localTransform,
                         RefRW<FindTarget> findTarget,
                         RefRW<Target> target
                     )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<FindTarget>,
                         RefRW<Target>
                     >()
                    )
            {
                findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.timer > 0f)
                {
                    continue;
                }

                target.ValueRW.targetEntity = Entity.Null;
                findTarget.ValueRW.timer = findTarget.ValueRO.maxTimer;
                hits.Clear();
                CollisionFilter collisonFilter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1 << RTSGame.UNITS_LAYER,
                    GroupIndex = 0
                };
                if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref hits,
                        collisonFilter))
                {
                    foreach (DistanceHit distanceHit in hits)
                    {
                        Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                        if (targetUnit.faction == findTarget.ValueRO.targetFaction)
                        {
                            target.ValueRW.targetEntity = distanceHit.Entity;
                            break;
                        }
                    }
                }
            }
        }
    }
}
