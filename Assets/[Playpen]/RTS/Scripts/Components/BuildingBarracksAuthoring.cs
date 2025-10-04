using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    public class BuildingBarracksAuthoring : MonoBehaviour
    {
        class Baker : Baker<BuildingBarracksAuthoring>
        {
            public override void Bake(BuildingBarracksAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new BuildingBarracks
                {
                    rallyPositionOffset = new float3(10, 0, 0),
                });
                AddBuffer<SpawnBuffer>(entity);
                
                AddComponent(entity, new BuildingBarracksUnitEnqueue());
                SetComponentEnabled<BuildingBarracksUnitEnqueue>(entity, false);
            }
        }
        
    }
    
    public struct BuildingBarracksUnitEnqueue : IComponentData, IEnableableComponent
    {
        public UnitScriptableObject.UnitType UnitType;
    }
    
    
    public struct BuildingBarracks : IComponentData
    {
        /// <summary>Timer counting up to the next spawn.</summary>
        public float timer;
        
        /// <summary>Interval in seconds between spawns.</summary>
        public float spawnDuration;
        
        public UnitScriptableObject.UnitType UnitType;

        public float3 rallyPositionOffset;

        public bool onUnitQueueChangedEventFlag;
    }

    
    [InternalBufferCapacity(10)]
    public struct SpawnBuffer : IBufferElementData
    {
        public UnitScriptableObject.UnitType UnitType;
    }
    
}

