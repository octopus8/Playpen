using Unity.Entities;
using UnityEngine;

public class DOTSFriendly : MonoBehaviour
{
    public class Baker : Baker<DOTSFriendly>
    {
        public override void Bake(DOTSFriendly authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Friendly>(entity);
        }
    }
}

public struct Friendly : IComponentData
{
}