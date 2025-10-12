using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for shoot target data. This component is added to entities that can be shot at.
    /// </summary>
    public class ShootTargetAuthoring : MonoBehaviour
    {
        /// <summary>Local position on the target where bullets should hit.</summary>
        [Tooltip("Local position on the target where bullets should hit.")] [SerializeField]
        private Transform hitLocalPosition;


        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Unity.Entities.Baker<ShootTargetAuthoring>
        {
            /// <summary>
            /// Adds the ShootTarget component to the entity with the specified parameters from the authoring component.
            /// </summary>
            public override void Bake(ShootTargetAuthoring authoring)
            {
                var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootTarget
                {
                    hitLocalPosition = authoring.hitLocalPosition.localPosition
                });
            }
        }
    }


    /// <summary>
    /// Component storing data for shoot targets, including the local position where bullets should hit.
    /// </summary>
    public struct ShootTarget : Unity.Entities.IComponentData
    {
        /// <summary>Local position on the target where bullets should hit.</summary>
        public float3 hitLocalPosition;
    }

}
