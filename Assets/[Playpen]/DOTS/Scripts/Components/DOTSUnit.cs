using Unity.Entities;
using UnityEngine;

public class DOTSUnit : MonoBehaviour
{
    public class Baker : Baker<DOTSUnit>
    {
        public override void Bake(DOTSUnit authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit());
        }
    }
}

public struct Unit : IComponentData
{
    
}
