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


        public class Baker : Unity.Entities.Baker<ShootTargetAuthoring>
        {
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


    public struct ShootTarget : Unity.Entities.IComponentData
    {
        /// <summary>Local position on the target where bullets should hit.</summary>
        public float3 hitLocalPosition;
    }

}
