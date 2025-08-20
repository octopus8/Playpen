using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<SelectedDOTS> selected in SystemAPI.Query<RefRO<SelectedDOTS>>().WithDisabled<SelectedDOTS>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            visualLocalTransform.ValueRW.Scale = 0f;
        }
        
        foreach (RefRO<SelectedDOTS> selected in SystemAPI.Query<RefRO<SelectedDOTS>>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
        }
    }

}
