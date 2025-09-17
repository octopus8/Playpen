using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for unit data. This component is added to all units.
    /// </summary>
    public class UnitAuthoring : MonoBehaviour
    {
        
        public class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit
                {
                });
            }
        }
    }

    
    public struct Unit : IComponentData
    {
    }
}
