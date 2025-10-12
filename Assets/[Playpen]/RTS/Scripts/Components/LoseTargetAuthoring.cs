using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for the LoseTarget ECS component.
    /// When an entity with this component moves further than the specified distance from its target,
    /// it will lose the target and stop pursuing it.
    /// </summary>
    public class LoseTargetAuthoring : MonoBehaviour
    {
        /// <summary> The distance at which the entity will lose its target. </summary>
        [Tooltip("The distance at which the entity will lose its target.")]
        [SerializeField] private float loseTargetDistance = 20f;
        
        
        /// <summary>
        /// Baker class to convert the authoring component to the ECS component.
        /// </summary>
        class Baker : Baker<LoseTargetAuthoring>
        {
            /// <summary>
            /// Adds the LoseTarget component to the entity with the specified distance from the authoring component.
            /// </summary>
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
    
    
    /// <summary>
    /// Component which specifies the distance at which an entity will lose its target.
    /// </summary>
    public struct LoseTarget : IComponentData
    {
        /// <summary> The distance at which the entity will lose its target. </summary>
        public float lostDistance;
    }
}

