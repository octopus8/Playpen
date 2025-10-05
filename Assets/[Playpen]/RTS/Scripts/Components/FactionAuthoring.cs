using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component to define which faction the entity belongs to. The `FindTargetSystem` uses this to find enemy targets, and `UnitSelection` uses this to find selected units.
    /// </summary>
    public class FactionAuthoring : MonoBehaviour
    {
        public FactionType factionType;

        public class Baker : Unity.Entities.Baker<FactionAuthoring>
        {
            public override void Bake(FactionAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent(entity, new Faction
                {
                    factionType = authoring.factionType
                });
            }
        }
        
    }
    
    struct Faction : Unity.Entities.IComponentData
    {
        public FactionType factionType;
    }
}
