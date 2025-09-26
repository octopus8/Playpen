using Unity.Entities;
using UnityEngine;

namespace RTS
{

    public class BuildingFriendlyHQAuthoring : MonoBehaviour
    {

        class Baker : Baker<BuildingFriendlyHQAuthoring>
        {
            public override void Bake(BuildingFriendlyHQAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingFriendlyHQ());
            }
        }
    
    
    }


    public struct BuildingFriendlyHQ : IComponentData
    {
    }
    
}

