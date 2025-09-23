using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    public class BuildingBarracksAuthoring : MonoBehaviour
    {
        /// <summary>Interval in seconds between spawns.</summary>
        [Tooltip("Interval in seconds between spawns.")]
        [SerializeField] private float spawnInterval = 2f;
    
        
        class Baker : Baker<BuildingBarracksAuthoring>
        {
            public override void Bake(BuildingBarracksAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingBarracks
                {
                    spawnDuration = authoring.spawnInterval,
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
        public UnitScriptableObject.UnitTypeID unitTypeID;
    }
    
    
    public struct BuildingBarracks : IComponentData
    {
        /// <summary>Timer counting up to the next spawn.</summary>
        public float timer;
        
        /// <summary>Interval in seconds between spawns.</summary>
        public float spawnDuration;
        
        public UnitScriptableObject.UnitTypeID unitTypeID;

        public float3 rallyPositionOffset;

        public bool onUnitQueueChanged;
    }

    
    [InternalBufferCapacity(10)]
    public struct SpawnBuffer : IBufferElementData
    {
        public UnitScriptableObject.UnitTypeID UnitTypeID;
    }
    
}

