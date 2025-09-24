using Unity.Entities;
using UnityEngine;


namespace RTS
{
    public class BuildingTypeAuthoring : MonoBehaviour
    {
        public BuildingScriptableObject.BuildingType buildingType;
        
        class Baker : Baker<BuildingTypeAuthoring>
        {
            public override void Bake(BuildingTypeAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingType
                {
                    buildingType = authoring.buildingType
                });
            }
        }
    }
    
    public struct BuildingType : IComponentData
    {
        public BuildingScriptableObject.BuildingType buildingType;
    }
    
}

