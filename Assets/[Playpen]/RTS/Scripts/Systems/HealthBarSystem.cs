using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RTS
{

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthBarSystem : ISystem
    {
        // Disable Burst for now due to Camera.main access
//        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Vector3 cameraForward = Vector3.zero;
            if (Camera.main != null)
            {
                cameraForward = Camera.main.transform.forward;
            }

            foreach (var (localTransform, healthBar) in
                     SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<HealthBar>
                     >())
            {
                LocalTransform parentLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
                if (localTransform.ValueRO.Scale == 1f)
                {
                    localTransform.ValueRW.Rotation =
                        parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
                }

                Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

                
                if (!health.onHealthChanged)
                {
                    continue;
                }
                
                
                float healthNormalized = (float)health.currentHealth / health.maxHealth;

                if (healthNormalized == 1f)
                {
                    localTransform.ValueRW.Scale = 0f;
                }
                else
                {
                    localTransform.ValueRW.Scale = 1f;
                }

                RefRW<PostTransformMatrix> barVisualPostTransformMatrix =
                    SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
                barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(new float3(healthNormalized, 1f, 1f));

            }
        }
    }
}
