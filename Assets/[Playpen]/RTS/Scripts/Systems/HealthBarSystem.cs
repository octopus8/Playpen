using Unity.Burst;
using Unity.Collections;
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
        private ComponentLookup<LocalTransform> _localTransformComponentLookup;
        private ComponentLookup<Health> _healthComponentLookup;
        private ComponentLookup<PostTransformMatrix> _postTransformMatrixComponentLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _localTransformComponentLookup =  state.GetComponentLookup<LocalTransform>();  
            _healthComponentLookup = state.GetComponentLookup<Health>(true);
            _postTransformMatrixComponentLookup = state.GetComponentLookup<PostTransformMatrix>(false);
        }

        
        /// <summary>
        /// Updates health bars to face the camera and reflect the current health of entities.
        /// </summary>
        // Disable Burst for now due to Camera.main access
//        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Vector3 cameraForward = Vector3.zero;
            if (Camera.main != null)
            {
                cameraForward = Camera.main.transform.forward;
            }
            
            _localTransformComponentLookup.Update(ref state);
            _healthComponentLookup.Update(ref state);
            _postTransformMatrixComponentLookup.Update(ref state);
            
            HealthBarJob job = new HealthBarJob
            {
                cameraForward = cameraForward,
                localTransformLookup = _localTransformComponentLookup,
                healthLookup = _healthComponentLookup,
                postTransformMatrixLookup = _postTransformMatrixComponentLookup,
            };
            job.ScheduleParallel();
            
        }
    }
    
    
    [BurstCompile]
    public partial struct HealthBarJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> localTransformLookup;
        [ReadOnly] public ComponentLookup<Health> healthLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> postTransformMatrixLookup;
        
        public float3 cameraForward;
        
        
        public void Execute(in HealthBar healthBar, Entity entity)
        {
            RefRW<LocalTransform> localTransform = localTransformLookup.GetRefRW(entity);
            LocalTransform parentLocalTransform = localTransformLookup[healthBar.healthEntity];
            if (localTransform.ValueRO.Scale == 1f)
            {
                localTransform.ValueRW.Rotation =
                    parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }
                
            Health health = healthLookup[healthBar.healthEntity];
            
            if (!health.onHealthChanged)
            {
                return;
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
                postTransformMatrixLookup.GetRefRW(healthBar.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(new float3(healthNormalized, 1f, 1f));
        }
    }
    
}
