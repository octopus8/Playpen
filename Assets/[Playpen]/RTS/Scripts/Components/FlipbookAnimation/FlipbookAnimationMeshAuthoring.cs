using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class FlipbookAnimationMeshAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject mesh;
        
        class Baker : Baker<FlipbookAnimationMeshAuthoring>
        {
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

    public struct FlipbookAnimationMesh : IComponentData
    {
        public Entity mesh;
    }
}
