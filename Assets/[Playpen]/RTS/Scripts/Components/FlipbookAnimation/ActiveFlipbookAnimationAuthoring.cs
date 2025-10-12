using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;


namespace RTS
{

    /// <summary>
    /// Authoring component for the ActiveFlipbookAnimation ECS component.
    /// </summary>
    public class ActiveFlipbookAnimationAuthoring : MonoBehaviour
    {
        /// <summary> The starting animation type for the entity. </summary>
        [Tooltip("The starting animation type for the entity.")]
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType startingAnimation = FlipbookAnimationScriptableObject.AnimationType.SoldierIdle;
        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<ActiveFlipbookAnimationAuthoring>
        {
            /// <summary>
            /// Adds the ActiveFlipbookAnimation component to the entity with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(ActiveFlipbookAnimationAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ActiveFlipbookAnimation
                {
                    nextAnimation = authoring.startingAnimation,
                });
            }
        }
    }


    /// <summary>
    /// Component storing data for active flipbook animations, including current frame, timers, and animation types.
    /// </summary>
    public struct ActiveFlipbookAnimation : IComponentData
    {
        /// <summary>Current frame index of the animation.</summary>
        public int frame;
        
        /// <summary>Timer tracking time elapsed for frame changes.</summary>
        public float frameTimer;
        
        /// <summary>Type of the currently active animation.</summary>
        public FlipbookAnimationScriptableObject.AnimationType activeAnimation;
        
        /// <summary>Type of the next animation to transition to.</summary>
        public FlipbookAnimationScriptableObject.AnimationType nextAnimation;
    }
}
