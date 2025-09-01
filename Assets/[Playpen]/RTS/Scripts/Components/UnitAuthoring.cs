using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for unit data. This component is added to all units.
    /// </summary>
    public class UnitAuthoring : MonoBehaviour
    {
        /// <summary>Faction of the unit.</summary>
        [Tooltip("Faction of the unit.")]
        [SerializeField] private Faction faction;

        
        public class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Unit
                {
                    faction = authoring.faction,
                });
            }
        }
    }

    
    public struct Unit : IComponentData
    {
        /// <summary>Faction of the unit.</summary>
        public Faction faction;
    }
}
