using Unity.Entities;
using UnityEngine;


namespace RTS
{

    public class FriendlyAuthoring : MonoBehaviour
    {
        public class Baker : Baker<FriendlyAuthoring>
        {
            public override void Bake(FriendlyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Friendly>(entity);
            }
        }
    }

    public struct Friendly : IComponentData
    {
    }
}
