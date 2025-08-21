using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

/// <summary>
/// DOTS system to manage the visual representation of entity selection.
/// Sets the scale of the visual representation of selected and not selected entities.
/// Selected entities are scaled to their specified scale, while not selected entities are scaled to zero.
/// This effectively hides not selected entities from view.
/// </summary>
partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Set the scale of the visual representation of not selected entities to zero,
        // effectively hiding them from view.
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            visualLocalTransform.ValueRW.Scale = 0f;
        }
        
        // Set the scale of the visual representation of selected entities to their specified scale,
        // making them visible in the scene.
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
        }
    }
}
