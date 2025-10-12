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
        /// <summary>Lookup for LocalTransform components, allowing read and write access.</summary>
        private ComponentLookup<LocalTransform> _localTransformComponentLookup;
        
        /// <summary>Lookup for Health components, allowing read-only access.</summary>
        private ComponentLookup<Health> _healthComponentLookup;
        
        /// <summary>Lookup for PostTransformMatrix components, allowing read and write access.</summary>
        private ComponentLookup<PostTransformMatrix> _postTransformMatrixComponentLookup;
        
        
        /// <summary>
        /// OnCreate is called when the system is created.
        /// It initializes component lookups for LocalTransform, Health, and PostTransformMatrix components.
        /// </summary>
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
            // Get the camera's forward direction.
            Vector3 cameraForward = Vector3.zero;
            if (Camera.main != null)
            {
                cameraForward = Camera.main.transform.forward;
            }
            
            // Update the component lookups.
            _localTransformComponentLookup.Update(ref state);
            _healthComponentLookup.Update(ref state);
            _postTransformMatrixComponentLookup.Update(ref state);
            
            // Schedule the HealthBarJob to update health bars.
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
    
    
    /// <summary>
    /// Job that makes health bars face the camera and updates their visual representation based on the entity's health.
    /// </summary>
    [BurstCompile]
    public partial struct HealthBarJob : IJobEntity
    {
        /// <summary>Lookup for LocalTransform components, allowing read and write access.</summary>
        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> localTransformLookup;
        
        /// <summary>Lookup for Health components, allowing read-only access.</summary>
        [ReadOnly] public ComponentLookup<Health> healthLookup;
        
        /// <summary>Lookup for PostTransformMatrix components, allowing read and write access.</summary>
        [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> postTransformMatrixLookup;
        
        /// <summary>Camera's forward direction.</summary>
        public float3 cameraForward;
        
        
        /// <summary>
        /// Makes the health bar face the camera and updates its visual representation based on the entity's health.
        /// </summary>
        public void Execute(in HealthBar healthBar, Entity entity)
        {
            // Get a reference to the LocalTransform component of the health bar entity.
            RefRW<LocalTransform> localTransform = localTransformLookup.GetRefRW(entity);
            
            // Get the LocalTransform of the parent entity that the health bar is attached to.
            LocalTransform parentLocalTransform = localTransformLookup[healthBar.healthEntity];
            
            // If the health bar is not scaled down (i.e., it's visible), make it face the camera.
            if (localTransform.ValueRO.Scale == 1f)
            {
                localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }
            
            // If there are no health changes, skip updating the health bar.
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
            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = postTransformMatrixLookup.GetRefRW(healthBar.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(new float3(healthNormalized, 1f, 1f));
        }
    }
}
