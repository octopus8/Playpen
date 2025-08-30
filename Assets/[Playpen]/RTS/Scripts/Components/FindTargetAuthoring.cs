using UnityEngine;


namespace RTS
{

    public class FindTargetAuthoring : MonoBehaviour
    {
        [SerializeField] private float range = 8f;

        [SerializeField] private Faction targetFaction;

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
        public float range;
        public Faction targetFaction;
        public float timer;
        public float maxTimer;
    }
}
