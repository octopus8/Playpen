using Unity.Entities;
using UnityEngine;

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