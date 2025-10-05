using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Tag authoring component which tags the entity as a "unit".
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
