using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTS
{
    public class FlipbookAnimationDataHolderAuthoring : MonoBehaviour
    {
        public FlipbookAnimationSetScriptableObject animationSet;

        /// <summary>This material is used to avoid the warning thrown if a `RenderMeshUnmanaged` is created without a "materialForSubMesh".</summary>
        [Tooltip("This material is used to avoid the warning thrown if a `RenderMeshUnmanaged` is created without a \"materialForSubMesh\".")]
        public Material defaultMaterial;
        
        class Baker : Baker<FlipbookAnimationDataHolderAuthoring>
        {
            public override void Bake(FlipbookAnimationDataHolderAuthoring authoring)
            {
/*
                FlipbookAnimationDataHolder flipbookAnimationDataHolder = new FlipbookAnimationDataHolder();

                EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();


                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref BlobArray<FlipbookAnimationData> animationData = ref blobBuilder.ConstructRoot<BlobArray<FlipbookAnimationData>>();

                BlobBuilderArray<FlipbookAnimationData> blobBuilderArray = blobBuilder.Allocate(ref animationData, System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)).Length);

                int index = 0;
                foreach (FlipbookAnimationScriptableObject.AnimationType animationType in System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)))
                {
                    FlipbookAnimationScriptableObject animationScriptableObject = authoring.animationSet.GetAnimation(animationType);


                    BlobBuilderArray<BatchMeshID> blobArray = blobBuilder.Allocate(ref blobBuilderArray[index].batchMeshIDBlobArray, animationScriptableObject.frames.Length);

                    blobBuilderArray[index].frameDuration = animationScriptableObject.frameDuration;
                    blobBuilderArray[index].totalFrames = animationScriptableObject.frames.Length;

                    for (int i = 0; i < blobBuilderArray[index].totalFrames; i++)
                    {
                        blobArray[i] = entitiesGraphicsSystem.RegisterMesh(animationScriptableObject.frames[i]);
                    }
                    index++;
                }

                flipbookAnimationDataHolder.animationData = blobBuilder.CreateBlobAssetReference<BlobArray<FlipbookAnimationData>>(Allocator.Persistent);

                blobBuilder.Dispose();

                // Ensure the blob asset is tracked and disposed of properly.
                AddBlobAsset(ref flipbookAnimationDataHolder.animationData, out _);

                // Add the component to the entity.
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, flipbookAnimationDataHolder);
*/                
                FlipbookAnimationDataHolder flipbookAnimationDataHolder = new FlipbookAnimationDataHolder();
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                int index = 0;
                foreach (
                    FlipbookAnimationScriptableObject.AnimationType animationType
                    in
                    System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)))
                {
                    FlipbookAnimationScriptableObject animationScriptableObject = authoring.animationSet.GetAnimation(animationType);
                    for (int i = 0; i < animationScriptableObject.frames.Length; i++)
                    {
                        Mesh mesh = animationScriptableObject.frames[i];
                        Entity additionalEntity = CreateAdditionalEntity(TransformUsageFlags.None, true);
                        AddComponent(additionalEntity, new MaterialMeshInfo());
                        AddComponent(additionalEntity, new RenderMeshUnmanaged
                        {
                            materialForSubMesh = authoring.defaultMaterial,
                            mesh = mesh,
                        });
                        AddComponent(additionalEntity, new FlipbookAnimationDataHolderSubEntity
                        {
                            animationType = animationType,
                            frameIndex = i
                        });
                    }
                }
                AddComponent(entity, new FlipbookAnimationDataHolderObjectData
                {
                    animationSet = authoring.animationSet
                });
                AddComponent(entity, flipbookAnimationDataHolder);
            }
        }
    }
    
    public struct FlipbookAnimationDataHolderObjectData : IComponentData
    {
        public UnityObjectRef<FlipbookAnimationSetScriptableObject> animationSet;
    }

    
    public struct FlipbookAnimationDataHolderSubEntity : IComponentData
    {
        public FlipbookAnimationScriptableObject.AnimationType animationType;
        public int frameIndex;
    }

    
    public struct FlipbookAnimationDataHolder : IComponentData
    {
        public BlobAssetReference<BlobArray<FlipbookAnimationData>> animationData;
    }

    public struct FlipbookAnimationData
    {
        public float frameDuration;
        public int totalFrames;
        public BlobArray<int> intMeshIDBlobArray;
    }
}
