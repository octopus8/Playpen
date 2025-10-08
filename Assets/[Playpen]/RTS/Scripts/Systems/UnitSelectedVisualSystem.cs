using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


namespace RTS
{

    /// <summary>
    /// DOTS system to manage the visual representation of entity selection.
    /// Sets the scale of the visual representation of selected and not selected entities.
    /// Selected entities are scaled to their specified scale, while not selected entities are scaled to zero.
    /// This effectively hides not selected entities from view.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(EventResetSystem))]
    partial struct UnitSelectedVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Iterate over all entities with the Selected component, enabled or not.
            // Disabled components are included because the component is disabled
            // upon deselection, but we still need to handle the visual scaling.
            foreach (RefRO<UnitSelected> selected in SystemAPI.Query<RefRO<UnitSelected>>().WithPresent<UnitSelected>())
            {
                // If the entity has just been deselected, scale down the visual representation to zero.
                if (selected.ValueRO.onDeselected)
                {
                    RefRW<LocalTransform> visualLocalTransform =
                        SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                    visualLocalTransform.ValueRW.Scale = 0f;
                }

                // If the entity has just been selected, scale up the visual representation to its specified scale.
                if (selected.ValueRO.onSelected)
                {
                    RefRW<LocalTransform> visualLocalTransform =
                        SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                    visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
                }
            }
        }
    }
}
