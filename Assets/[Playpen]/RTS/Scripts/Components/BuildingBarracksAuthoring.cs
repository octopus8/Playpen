using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for the BuildingBarracks ECS component.
    /// </summary>
    public class BuildingBarracksAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<BuildingBarracksAuthoring>
        {
            /// <summary>
            /// Adds the BuildingBarracks component to the entity with default parameters.
            /// </summary>
            public override void Bake(BuildingBarracksAuthoring authoring)
            {
                // Add BuildingBarracks component with default parameters.
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingBarracks
                {
                    rallyPositionOffset = new float3(10, 0, 0),
                });
                
                // Add SpawnBuffer and BuildingBarracksUnitEnqueue components.
                AddBuffer<SpawnBuffer>(entity);
                AddComponent(entity, new BuildingBarracksUnitEnqueue());
                
                // Disable the BuildingBarracksUnitEnqueue component by default.
                SetComponentEnabled<BuildingBarracksUnitEnqueue>(entity, false);
            }
        }
    }
    
    
    /// <summary>
    /// Enableable component for enqueueing unit spawns in the barracks.
    /// This component is enabled when a unit is queued for spawning.
    /// </summary>
    public struct BuildingBarracksUnitEnqueue : IComponentData, IEnableableComponent
    {
        public UnitScriptableObject.UnitType UnitType;
    }
    
    
    /// <summary>
    /// Component storing data for the barracks building, including spawn timing and rally point offset.
    /// </summary>
    public struct BuildingBarracks : IComponentData
    {
        /// <summary>Timer counting up to the next spawn.</summary>
        public float timer;
        
        /// <summary>Interval in seconds between spawns.</summary>
        public float currentSpawnDuration;
     
        /// <summary>Type of unit to spawn.</summary>
        public UnitScriptableObject.UnitType spawnType;

        /// <summary>Offset from the barracks position for the rally point where spawned units will move to.</summary>
        public float3 rallyPositionOffset;

        /// <summary>Event flag indicating if the unit queue has changed (unit added or removed).</summary>
        public bool onUnitQueueChangedEventFlag;
    }

    
    /// <summary>
    /// Buffer element for storing units queued to be spawned by the barracks.
    /// </summary>
    [InternalBufferCapacity(10)]
    public struct SpawnBuffer : IBufferElementData
    {
        public UnitScriptableObject.UnitType unitType;
    }
}

