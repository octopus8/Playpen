using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;


namespace RTS
{

    public class ActiveFlipbookAnimationAuthoring : MonoBehaviour
    {
        public FlipbookAnimationScriptableObject.AnimationType startingAnimation = FlipbookAnimationScriptableObject.AnimationType.SoldierIdle;
        
        class Baker : Baker<ActiveFlipbookAnimationAuthoring>
        {
            public override void Bake(ActiveFlipbookAnimationAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                AddComponent(entity, new ActiveFlipbookAnimation
                {
                    nextAnimation = authoring.startingAnimation,
                });
            }
        }
    }


    
    public struct ActiveFlipbookAnimation : IComponentData
    {
        public int frame;
        public float frameTimer;
        public FlipbookAnimationScriptableObject.AnimationType activeAnimation;
        public FlipbookAnimationScriptableObject.AnimationType nextAnimation;
    }
}
