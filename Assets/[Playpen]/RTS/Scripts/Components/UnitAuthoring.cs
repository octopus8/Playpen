using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Tag authoring component which tags the entity as a "unit".
    /// </summary>
    public class UnitAuthoring : MonoBehaviour
    {
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Baker<UnitAuthoring>
        {
            /// <summary>
            /// Adds the Unit tag component to the entity.
            /// </summary>
            public override void Bake(UnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit
                {
                });
            }
        }
    }
    
    
    /// <summary>
    /// Tag component which marks an entity as a "unit".
    /// </summary>
    public struct Unit : IComponentData
    {
    }
}
