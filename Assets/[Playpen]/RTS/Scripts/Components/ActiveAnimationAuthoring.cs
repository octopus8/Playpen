using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;


namespace RTS
{

    public class ActiveAnimationAuthoring : MonoBehaviour
    {
        public FlipbookAnimationScriptableObject flipbookAnimation;
        
        class Baker : Baker<ActiveAnimationAuthoring>
        {
            public override void Bake(ActiveAnimationAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                AddComponent(entity, new ActiveAnimation
                {
                    frame0 = entitiesGraphicsSystem.RegisterMesh(authoring.flipbookAnimation.frames[0]),
                    frame1 = entitiesGraphicsSystem.RegisterMesh(authoring.flipbookAnimation.frames[1]),
                    totalFrames = authoring.flipbookAnimation.frames.Length,
                    frameDuration = authoring.flipbookAnimation.frameDuration,
                });
            }
        }
    }

    
    public struct ActiveAnimation : IComponentData
    {
        public int frame;
        public int totalFrames;
        public float frameTime;
        public float frameDuration;
        public BatchMeshID frame0;
        public BatchMeshID frame1;
    }
}
