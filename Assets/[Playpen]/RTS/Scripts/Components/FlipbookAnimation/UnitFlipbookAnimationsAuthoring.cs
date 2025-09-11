using Unity.Entities;
using UnityEngine;

namespace RTS
{

    public class UnitFlipbookAnimationsAuthoring : MonoBehaviour
    {
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType idleAnimation;
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType walkAnimation;
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType aimAnimation;
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType shootAnimation;
        [SerializeField] private FlipbookAnimationScriptableObject.AnimationType meleeAttackAnimation;
        
        class Baker : Baker<UnitFlipbookAnimationsAuthoring>
        {
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

    public struct UnitFlipbookAnimations : IComponentData
    {
        public FlipbookAnimationScriptableObject.AnimationType idleAnimation;
        public FlipbookAnimationScriptableObject.AnimationType walkAnimation;
        public FlipbookAnimationScriptableObject.AnimationType aimAnimation;
        public FlipbookAnimationScriptableObject.AnimationType shootAnimation;
        public FlipbookAnimationScriptableObject.AnimationType meleeAttackAnimation;
    }
    
}
