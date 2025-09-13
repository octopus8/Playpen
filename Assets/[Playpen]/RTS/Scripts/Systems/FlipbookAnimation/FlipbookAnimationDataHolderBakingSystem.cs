using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace RTS
{
    /// <summary>
    /// This baking system creates the BlobAssetReference for the FlipbookAnimationDataHolder component.
    /// It collects the mesh IDs from the sub-entities created in the Baker and organizes them
    /// into the BlobAssetReference structure based on the animation type and frame index.
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(PostBakingSystemGroup))]
    partial struct FlipbookAnimationDataHolderBakingSystem : ISystem
    {


        public void OnUpdate(ref SystemState state)
        {
            FlipbookAnimationSetScriptableObject animationSet = null;
            foreach (var
                         authoring
                     in
                     SystemAPI.Query<RefRO<FlipbookAnimationDataHolderObjectData>>())
            {
                animationSet = authoring.ValueRO.animationSet.Value;
            }
            
            
            Dictionary<FlipbookAnimationScriptableObject.AnimationType, int[]> blobAssetData = new Dictionary<FlipbookAnimationScriptableObject.AnimationType, int[]>();

            foreach (FlipbookAnimationScriptableObject.AnimationType animationType in System.Enum.GetValues(
                         typeof(FlipbookAnimationScriptableObject.AnimationType)))
            {
                FlipbookAnimationScriptableObject animationScriptableObject = animationSet.GetAnimation(animationType);
                blobAssetData[animationType] = new int[animationScriptableObject.frames.Length];
            }

            foreach (var (
                         animationDataSubEntity,
                         materialMeshInfo)
                     in
                     SystemAPI.Query<
                         RefRO<FlipbookAnimationDataHolderSubEntity>,
                         RefRO<MaterialMeshInfo>>())
            {
                blobAssetData[animationDataSubEntity.ValueRO.animationType][animationDataSubEntity.ValueRO.frameIndex] =
                    materialMeshInfo.ValueRO.Mesh;
            }
            
            foreach (var
                         animationDataHolder
                     in
                     SystemAPI.Query<RefRW<FlipbookAnimationDataHolder>>())
            {
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref BlobArray<FlipbookAnimationData> animationData = ref blobBuilder.ConstructRoot<BlobArray<FlipbookAnimationData>>();

                BlobBuilderArray<FlipbookAnimationData> blobBuilderArray = blobBuilder.Allocate(ref animationData, System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)).Length);

                int index = 0;
                foreach (FlipbookAnimationScriptableObject.AnimationType animationType in System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)))
                {
                    FlipbookAnimationScriptableObject animationScriptableObject = animationSet.GetAnimation(animationType);


                    BlobBuilderArray<int> blobArray = blobBuilder.Allocate<int>(ref blobBuilderArray[index].intMeshIDBlobArray, animationScriptableObject.frames.Length);

                    blobBuilderArray[index].frameDuration = animationScriptableObject.frameDuration;
                    blobBuilderArray[index].totalFrames = animationScriptableObject.frames.Length;

                    for (int i = 0; i < blobBuilderArray[index].totalFrames; i++)
                    {
                        blobArray[i] = blobAssetData[animationType][i];
                    }
                    index++;
                }

                animationDataHolder.ValueRW.animationData = blobBuilder.CreateBlobAssetReference<BlobArray<FlipbookAnimationData>>(Allocator.Persistent);

                blobBuilder.Dispose();
            }
        }

        
    }
}
