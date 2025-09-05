using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class LoseTargetAuthoring : MonoBehaviour
    {
        [SerializeField] private float loseTargetDistance = 20f;
        class Baker : Baker<LoseTargetAuthoring>
        {
            public override void Bake(LoseTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new LoseTarget
                {
                    lostDistance = authoring.loseTargetDistance
                });
            }
        }
    }
    
    public struct LoseTarget : IComponentData
    {
        public float lostDistance;
    }
}

