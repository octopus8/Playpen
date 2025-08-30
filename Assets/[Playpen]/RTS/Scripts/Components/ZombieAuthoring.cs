using UnityEngine;

namespace RTS
{
    public class ZombieAuthoring : MonoBehaviour
    {
        public class Baker : Unity.Entities.Baker<ZombieAuthoring>
        {
            public override void Bake(ZombieAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent<Zombie>(entity);
            }
        }
    }


    public struct Zombie : Unity.Entities.IComponentData
    {
    }    
}

