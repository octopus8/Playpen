using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class UnitSpawnerAuthoring : MonoBehaviour
    {
        /// <summary>Interval in seconds between spawns.</summary>
        [Tooltip("Interval in seconds between spawns.")]
        [SerializeField] private float spawnInterval = 2f;
    
        
        class Baker : Baker<UnitSpawnerAuthoring>
        {
            public override void Bake(UnitSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitSpawner
                {
                    spawnInterval = authoring.spawnInterval,
                });
                DynamicBuffer<SpawnBuffer> spawnBuffer = AddBuffer<SpawnBuffer>(entity);
                spawnBuffer.Add(new SpawnBuffer { unitType = UnitScriptableObject.UnitType.Soldier });
                spawnBuffer.Add(new SpawnBuffer { unitType = UnitScriptableObject.UnitType.Soldier });
                spawnBuffer.Add(new SpawnBuffer { unitType = UnitScriptableObject.UnitType.Scout });
            }
        }
        
    }
    
    
    public struct UnitSpawner : IComponentData
    {
        /// <summary>Timer counting up to the next spawn.</summary>
        public float timer;
        
        /// <summary>Interval in seconds between spawns.</summary>
        public float spawnInterval;
    }

    
    [InternalBufferCapacity(10)]
    public struct SpawnBuffer : IBufferElementData
    {
        public UnitScriptableObject.UnitType unitType;
    }
    
}

