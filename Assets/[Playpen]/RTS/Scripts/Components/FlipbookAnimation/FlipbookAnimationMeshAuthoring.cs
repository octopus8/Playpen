using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for the FlipbookAnimationMesh ECS component.
    /// </summary>
    public class FlipbookAnimationMeshAuthoring : MonoBehaviour
    {
        /// <summary> The mesh GameObject used for flipbook animations. </summary>
        [Tooltip("The mesh GameObject used for flipbook animations.")]
        [SerializeField] private GameObject mesh;

        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class Baker : Baker<FlipbookAnimationMeshAuthoring>
        {
            /// <summary>
            /// Adds the FlipbookAnimationMesh component to the entity with the specified mesh from the authoring component.
            /// </summary>
            public override void Bake(FlipbookAnimationMeshAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FlipbookAnimationMesh
                {
                    mesh = GetEntity(authoring.mesh, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    
    /// <summary>
    /// Component storing the mesh entity used for flipbook animations.
    /// </summary>
    public struct FlipbookAnimationMesh : IComponentData
    {
        /// <summary> The mesh Entity used for flipbook animations. </summary>
        public Entity mesh;
    }
}
