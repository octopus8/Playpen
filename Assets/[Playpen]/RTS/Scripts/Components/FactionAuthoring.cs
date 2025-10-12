using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component to define which faction the entity belongs to.
    /// The `FindTargetSystem` uses this to find enemy targets, and `UnitSelection` uses this to find selected units.
    /// </summary>
    public class FactionAuthoring : MonoBehaviour
    {
        /// <summary>The faction type of the entity.</summary>
        [Tooltip("The faction type of the entity.")]
        [SerializeField] private FactionType factionType;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Unity.Entities.Baker<FactionAuthoring>
        {
            /// <summary>
            /// Adds the Faction component to the entity with the faction type specified in the authoring component.
            /// </summary>
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
    
    
    /// <summary>
    /// Component which specifies the faction an entity belongs to.
    /// </summary>
    struct Faction : Unity.Entities.IComponentData
    {
        /// <summary>The faction type of the entity.</summary>
        public FactionType factionType;
    }
}
