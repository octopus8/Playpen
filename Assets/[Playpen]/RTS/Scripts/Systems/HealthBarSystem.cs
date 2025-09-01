using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// System that updates health bars to face the camera and reflect the current health of entities.
    /// This system runs in the LateSimulationSystemGroup to ensure events can be sent before updating health bars.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthBarSystem : ISystem
    {
        /// <summary>
        /// Updates health bars to face the camera and reflect the current health of entities.
        /// </summary>
        // Disable Burst for now due to Camera.main access
//        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with LocalTransform and HealthBar components.
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
                // Rotate the health bar to face the camera, but only if it's not scaled down to zero.
                LocalTransform parentLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
                if (localTransform.ValueRO.Scale == 1f)
                {
                    localTransform.ValueRW.Rotation =
                        parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
                }
                
                // If there's no health change event, skip updating the health bar.
                Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);
                if (!health.onHealthChanged)
                {
                    continue;
                }
                
                // If health is full, scale down the health bar to zero to hide it.
                float healthNormalized = (float)health.currentHealth / health.maxHealth;
                if (healthNormalized == 1f)
                {
                    localTransform.ValueRW.Scale = 0f;
                }
                // Otherwise, ensure the health bar is fully visible.
                else
                {
                    localTransform.ValueRW.Scale = 1f;
                }

                // Update the health bar's visual representation based on the current health.
                RefRW<PostTransformMatrix> barVisualPostTransformMatrix =
                    SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
                barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(new float3(healthNormalized, 1f, 1f));
            }
        }
    }
}
