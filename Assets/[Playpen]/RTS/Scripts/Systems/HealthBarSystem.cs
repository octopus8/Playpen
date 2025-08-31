using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS
{

partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var healthBar in
                 SystemAPI.Query<
                     RefRW<HealthBar>
                 >())
        {
            Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);
            float healthNormalized = (float)health.currentHealth / health.maxHealth;

            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(new float3(healthNormalized, 1f, 1f));
            
        }
    }
}
}
