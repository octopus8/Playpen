using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component to mark an entity as friendly.
    /// Friendly entities are on the player's team.
    /// </summary>
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
