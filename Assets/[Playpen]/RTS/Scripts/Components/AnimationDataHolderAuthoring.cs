using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTS
{
    public class AnimationDataHolderAuthoring : MonoBehaviour
    {
        public FlipbookAnimationScriptableObject soldierIdleAnimation;
        public FlipbookAnimationScriptableObject soldierWalkAnimation;
        
        class Baker : Baker<AnimationDataHolderAuthoring>
        {
            public override void Bake(AnimationDataHolderAuthoring authoring)
            {
                AnimationDataHolder animationDataHolder = new AnimationDataHolder();
                EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                
                
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref AnimationData animationData = ref blobBuilder.ConstructRoot<AnimationData>();
                animationData.frameDuration = authoring.soldierIdleAnimation.frameDuration;
                animationData.totalFrames = authoring.soldierIdleAnimation.frames.Length;
                BlobBuilderArray<BatchMeshID> blobArray = blobBuilder.Allocate(ref animationData.batchMeshIDBlobArray, animationData.totalFrames);
                for (int i = 0; i < animationData.totalFrames; i++)
                {
                    blobArray[i] = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdleAnimation.frames[i]);
                }
                animationDataHolder.soldierIdleAnimationData = blobBuilder.CreateBlobAssetReference<AnimationData>(Allocator.Persistent);
                blobBuilder.Dispose();
                
                // Ensure the blob asset is tracked and disposed of properly.
                AddBlobAsset(ref animationDataHolder.soldierIdleAnimationData, out _);
                
                // Add the component to the entity.
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, animationDataHolder);
            }
        }
    }

    
    public struct AnimationDataHolder : IComponentData
    {
        public BlobAssetReference<AnimationData> soldierIdleAnimationData;
        public BlobAssetReference<AnimationData> soldierWalkAnimationData;
    }

    public struct AnimationData
    {
        public float frameDuration;
        public int totalFrames;
        public BlobArray<BatchMeshID> batchMeshIDBlobArray;
    }
}
