using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for the BuildingType ECS component.
    /// </summary>
    public class BuildingTypeAuthoring : MonoBehaviour
    {
        /// <summary> The type of building this entity represents. </summary>
        [Tooltip("The type of building this entity represents.")]
        [SerializeField] private BuildingScriptableObject.BuildingType buildingType;

        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<BuildingTypeAuthoring>
        {
            /// <summary>
            /// Adds the BuildingType component to the entity.
            /// </summary>
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
    
    
    /// <summary>
    /// Component which specifies the type of building an entity represents.
    /// </summary>
    public struct BuildingType : IComponentData
    {
        /// <summary> The type of building this entity represents. </summary>
        public BuildingScriptableObject.BuildingType buildingType;
    }
}

