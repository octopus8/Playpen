using Unity.Entities;
using UnityEngine;

public class DOTSTarget : MonoBehaviour
{
    
    public class Baker : Baker<DOTSTarget>
    {
        public override void Bake(DOTSTarget authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new Target());
        }
    }
    
    
}


public struct Target : Unity.Entities.IComponentData
{
    public Entity targetEntity;
}
