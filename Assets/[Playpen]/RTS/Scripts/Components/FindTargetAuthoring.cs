using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Authoring component for finding targets.
    /// This component is added to units that can find and attack targets.
    /// </summary>
    public class FindTargetAuthoring : MonoBehaviour
    {
        /// <summary>Range to search for targets.</summary>
        [Tooltip("Range to search for targets.")]
        [SerializeField] private float range = 8f;

        /// <summary>The faction to target.</summary>
        [Tooltip("The faction to target.")]
        [SerializeField] private FactionType targetFactionType;

        /// <summary>Maximum time between target searches in seconds.</summary>
        [Tooltip("Maximum time between target searches in seconds.")]
        [SerializeField] private float maxTimerSeconds = 0.2f;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Unity.Entities.Baker<FindTargetAuthoring>
        {
            /// <summary>
            /// Adds the FindTarget component to the entity with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(FindTargetAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget
                {
                    range = authoring.range,
                    TargetFactionType = authoring.targetFactionType,
                    maxTimer = authoring.maxTimerSeconds,
                });
            }
        }
    }


    /// <summary>
    /// Component storing data for finding targets.
    /// </summary>
    public struct FindTarget : Unity.Entities.IComponentData
    {
        /// <summary>Range to search for targets.</summary>
        public float range;
        
        /// <summary>The faction to target.</summary>
        public FactionType TargetFactionType;
        
        /// <summary>Timer to track time between target searches.</summary>
        public float timer;
        
        /// <summary>Maximum time between target searches in seconds.</summary>
        public float maxTimer;
    }
}
