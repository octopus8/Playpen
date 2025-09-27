using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTS
{
    public class FlipbookAnimationDataHolderAuthoring : MonoBehaviour
    {
        /// <summary>The set of flipbook animations for all entities.</summary>
        [Tooltip("The set of flipbook animations for all entities.")]
        public FlipbookAnimationSetScriptableObject animationSet;

        /// <summary>This material is used to avoid the warning thrown if a `RenderMeshUnmanaged` is created without a "materialForSubMesh".</summary>
        [Tooltip("This material is used to avoid the warning thrown if a `RenderMeshUnmanaged` is created without a \"materialForSubMesh\".")]
        public Material defaultMaterial;
        
        class Baker : Baker<FlipbookAnimationDataHolderAuthoring>
        {
            /// <summary>
            /// This method creates bake time only sub-entities for each frame of each animation type defined in the FlipbookAnimationSetScriptableObject.
            /// Each sub-entity is assigned a RenderMeshUnmanaged component with the corresponding mesh and a MaterialMeshInfo component.
            /// The main entity is assigned a FlipbookAnimationDataHolderObjectData component referencing the animation set ScriptableObject
            /// and a FlipbookAnimationDataHolder component to hold the BlobAssetReference for the animation data. This data will be populated
            /// in the FlipbookAnimationDataHolderBakingSystem.
            /// By adding the mesh to a RenderMeshUnmanaged component, the mesh is automatically registered and assigned a unique mesh ID,
            /// which is used in the BlobAssetReference for efficient access during runtime.
            /// </summary>
            public override void Bake(FlipbookAnimationDataHolderAuthoring authoring)
            {
                // Iterate through all animation types.
                FlipbookAnimationDataHolder flipbookAnimationDataHolder = new FlipbookAnimationDataHolder();
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                foreach (
                    FlipbookAnimationScriptableObject.AnimationType animationType
                    in
                    System.Enum.GetValues(typeof(FlipbookAnimationScriptableObject.AnimationType)))
                {
                    // Iterate through all frames.
                    FlipbookAnimationScriptableObject animationScriptableObject = authoring.animationSet.GetAnimation(animationType);
                    for (int i = 0; i < animationScriptableObject.frames.Length; i++)
                    {
                        // Create an additional bake-time only entity for each mesh.
                        Mesh mesh = animationScriptableObject.frames[i];
                        Entity additionalEntity = CreateAdditionalEntity(TransformUsageFlags.None, true);
                        
                        // Add component containing the frame mesh.
                        AddComponent(additionalEntity, new MaterialMeshInfo());
                        AddComponent(additionalEntity, new RenderMeshUnmanaged
                        {
                            materialForSubMesh = authoring.defaultMaterial,
                            mesh = mesh,
                        });

                        // Add component containing the animation type and frame index information.
                        AddComponent(additionalEntity, new FlipbookAnimationDataHolderSubEntity
                        {
                            animationType = animationType,
                            frameIndex = i
                        });
                    }
                }
                
                // Add component containing the reference to the animation set ScriptableObject.
                AddComponent(entity, new FlipbookAnimationDataHolderObjectData
                {
                    animationSet = authoring.animationSet
                });

                // Add component to hold the BlobAssetReference for the animation data, which will be created in the FlipbookAnimationDataHolderBakingSystem.
                AddComponent(entity, flipbookAnimationDataHolder);
            }
        }
    }
    
    
    /// <summary>
    /// Holds a reference to the FlipbookAnimationSetScriptableObject which contains all animations.
    /// </summary>
    /// <remarks>
    /// This is later used by the FlipbookAnimationDataHolderBakingSystem to access the animation set
    /// and create the BlobAssetReference for the FlipbookAnimationDataHolder component.
    /// </remarks>
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
