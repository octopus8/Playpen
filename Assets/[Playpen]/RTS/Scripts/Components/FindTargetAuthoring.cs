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
        [SerializeField] private Faction targetFaction;

        /// <summary>Maximum time between target searches in seconds.</summary>
        [Tooltip("Maximum time between target searches in seconds.")]
        [SerializeField] private float maxTimerSeconds = 0.2f;

        
        public class Baker : Unity.Entities.Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget
                {
                    range = authoring.range,
                    targetFaction = authoring.targetFaction,
                    maxTimer = authoring.maxTimerSeconds,
                });
            }
        }
    }


    public struct FindTarget : Unity.Entities.IComponentData
    {
        /// <summary>Range to search for targets.</summary>
        public float range;
        /// <summary>The faction to target.</summary>
        public Faction targetFaction;
        /// <summary>Timer to track time between target searches.</summary>
        public float timer;
        /// <summary>Maximum time between target searches in seconds.</summary>
        public float maxTimer;
    }
}
