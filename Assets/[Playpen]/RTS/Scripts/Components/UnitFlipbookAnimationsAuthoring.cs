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
    }
    
}
