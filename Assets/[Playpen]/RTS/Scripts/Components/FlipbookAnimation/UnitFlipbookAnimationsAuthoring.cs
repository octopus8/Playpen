using Unity.Entities;
using UnityEngine;

namespace RTS
{

    /// <summary>
    /// Authoring component for the UnitFlipbookAnimations ECS component.
    /// This component holds the flipbook animations for a unit.
    /// </summary>
    public class UnitFlipbookAnimationsAuthoring : MonoBehaviour
    {
        /// <summary> The idle animation. </summary>
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType idleAnimation;
        
        /// <summary> The walk animation. </summary>
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType walkAnimation;
        
        /// <summary> The aim animation. </summary>
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType aimAnimation;
        
        /// <summary> The shoot animation. </summary>
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType shootAnimation;
        
        /// <summary> The melee attack animation. </summary>
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType meleeAttackAnimation;
    
        
        /// <summary>
        /// Baker class to convert the authoring component to the ECS component.
        /// </summary>
        class Baker : Baker<UnitFlipbookAnimationsAuthoring>
        {
            /// <summary>
            /// Adds the UnitFlipbookAnimations component to the entity with the specified animations from the authoring component.
            /// </summary>
            public override void Bake(UnitFlipbookAnimationsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitFlipbookAnimations
                {
                    idleAnimation = authoring.idleAnimation,
                    walkAnimation = authoring.walkAnimation,
                    aimAnimation = authoring.aimAnimation,
                    shootAnimation = authoring.shootAnimation,
                    meleeAttackAnimation = authoring.meleeAttackAnimation
                });
            }
        }
    }

    
    /// <summary>
    /// Component that holds the flipbook animations for a unit.
    /// </summary>
    public struct UnitFlipbookAnimations : IComponentData
    {
        /// <summary> The idle animation. </summary>
        public FlipbookAnimationScriptableObject.AnimationType idleAnimation;
        
        /// <summary> The walk animation. </summary>
        public FlipbookAnimationScriptableObject.AnimationType walkAnimation;
        
        /// <summary> The aim animation. </summary>
        public FlipbookAnimationScriptableObject.AnimationType aimAnimation;
        
        /// <summary> The shoot animation. </summary>
        public FlipbookAnimationScriptableObject.AnimationType shootAnimation;
        
        /// <summary> The melee attack animation. </summary>
        public FlipbookAnimationScriptableObject.AnimationType meleeAttackAnimation;
    }
}
