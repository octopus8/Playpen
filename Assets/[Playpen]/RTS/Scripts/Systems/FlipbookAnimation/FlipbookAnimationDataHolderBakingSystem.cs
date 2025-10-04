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
            // Get a reference to the set of animation data. This reference is contained in the FlipbookAnimationDataHolderObjectData component
            // added to the main entity in the Baker.
            // Query returns an enumerable so we can use a foreach to get the first (and only) element.
            FlipbookAnimationSetScriptableObject animationSet = null;
            foreach (var
                         authoring
                     in
                     SystemAPI.Query<RefRO<FlipbookAnimationDataHolderObjectData>>())
            {
                animationSet = authoring.ValueRO.animationSet.Value;
            }
            
            // Create a dictionary to map animation types to the meshes corresponding to each frame of that animation.
            // The key is the animation type and the value is an array of mesh IDs corresponding to the frames of that animation.
            Dictionary<FlipbookAnimationScriptableObject.AnimationType, int[]> blobAssetData = new Dictionary<FlipbookAnimationScriptableObject.AnimationType, int[]>();
            
            // Initialize the dictionary with empty arrays for each animation type.
            foreach (FlipbookAnimationScriptableObject.AnimationType animationType in System.Enum.GetValues(
                         typeof(FlipbookAnimationScriptableObject.AnimationType)))
            {
                FlipbookAnimationScriptableObject animationScriptableObject = animationSet.GetAnimation(animationType);
                blobAssetData[animationType] = new int[animationScriptableObject.frames.Length];
            }

            // Iterate through all the mesh sub-entities created in the Baker.
            // Populate the dictionary with the mesh IDs from the sub-entities.
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
            
            // Get a reference to the main entity's FlipbookAnimationDataHolder component and populate its BlobAssetReference
            // with the data from the dictionary.
            // Query returns an enumerable so we can use a foreach to get the first (AND ONLY) element.
            // Note: I think it would be better to store the value in a local variable like as is done with `animationSet` above.
            foreach (var
                         animationDataHolder
                     in
                     SystemAPI.Query<RefRW<FlipbookAnimationDataHolder>>())
            {
                // Create a BlobBuilder.
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                
                // Construct a root reference.
                ref BlobArray<FlipbookAnimationData> animationData = ref blobBuilder.ConstructRoot<BlobArray<FlipbookAnimationData>>();

                // Use the root reference to allocate an array for the FlipbookAnimationData corresponding to each animation type.
                BlobBuilderArray<FlipbookAnimationData> blobFlipbookAnimationData = blobBuilder.Allocate(ref animationData, System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)).Length);

                // Iterate through each animation type and populate the array.
                int index = 0;
                foreach (FlipbookAnimationScriptableObject.AnimationType animationType in System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)))
                {
                    // Get the animation ScriptableObject for the current animation type.
                    FlipbookAnimationScriptableObject animationScriptableObject = animationSet.GetAnimation(animationType);
                    
                    // Use the BlobBuilder to allocate an array for the mesh IDs corresponding to each frame of the animation.
                    BlobBuilderArray<int> blobArray = blobBuilder.Allocate<int>(ref blobFlipbookAnimationData[index].intMeshIDBlobArray, animationScriptableObject.frames.Length);

                    // Populate the FlipbookAnimationData for the current animation type.
                    blobFlipbookAnimationData[index].frameDuration = animationScriptableObject.frameDuration;
                    blobFlipbookAnimationData[index].totalFrames = animationScriptableObject.frames.Length;

                    // Iterate through each frame of the animation and populate the mesh ID array in the BlobAssetReference.
                    for (int i = 0; i < blobFlipbookAnimationData[index].totalFrames; i++)
                    {
                        blobArray[i] = blobAssetData[animationType][i];
                    }
                    index++;
                }

                // Create the BlobAssetReference and assign it to the FlipbookAnimationDataHolder component.
                animationDataHolder.ValueRW.animationData = blobBuilder.CreateBlobAssetReference<BlobArray<FlipbookAnimationData>>(Allocator.Persistent);

                // Dispose of the BlobBuilder to free temporary memory.
                blobBuilder.Dispose();
            }
        }

        
    }
}
