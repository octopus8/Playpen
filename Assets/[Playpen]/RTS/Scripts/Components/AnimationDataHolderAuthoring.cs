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
        
        class Baker : Baker<AnimationDataHolderAuthoring>
        {
            public override void Bake(AnimationDataHolderAuthoring authoring)
            {
                AnimationDataHolder animationDataHolder = new AnimationDataHolder();
                EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                
                
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref BlobArray<AnimationData> animationData = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();
                
                BlobBuilderArray<AnimationData> blobBuilderArray = blobBuilder.Allocate(ref animationData, 2);


                {
                    BlobBuilderArray<BatchMeshID> blobArray = blobBuilder.Allocate(ref blobBuilderArray[0].batchMeshIDBlobArray, authoring.soldierIdleAnimation.frames.Length);
                    
                    blobBuilderArray[0].frameDuration = authoring.soldierIdleAnimation.frameDuration;
                    blobBuilderArray[0].totalFrames = authoring.soldierIdleAnimation.frames.Length;
                    
                    for (int i = 0; i < blobBuilderArray[0].totalFrames; i++)
                    {
                        blobArray[i] = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdleAnimation.frames[i]);
                    }
                }
                
                animationDataHolder.animationData = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);
                
                blobBuilder.Dispose();
                
                // Ensure the blob asset is tracked and disposed of properly.
                AddBlobAsset(ref animationDataHolder.animationData, out _);
                
                // Add the component to the entity.
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, animationDataHolder);
            }
        }
    }

    
    public struct AnimationDataHolder : IComponentData
    {
        public BlobAssetReference<BlobArray<AnimationData>> animationData;
    }

    public struct AnimationData
    {
        public float frameDuration;
        public int totalFrames;
        public BlobArray<BatchMeshID> batchMeshIDBlobArray;
    }
}
