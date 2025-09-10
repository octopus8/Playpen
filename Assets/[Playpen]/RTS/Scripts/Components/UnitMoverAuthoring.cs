using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Authoring component for unit movement data. This component is added to all units.
    /// </summary>
    public class UnitMoverAuthoring : MonoBehaviour
    {
        /// <summary> Unit movement speed, in meters per second. </summary>
        [Tooltip("Unit movement speed.")]
        [SerializeField] private float moveSpeedMPS = 5f;

        /// <summary> Unit rotation speed, in radians per second. </summary>
        [Tooltip("Unit rotation speed.")]
        [SerializeField] private float rotationSpeedRPS = 10f;


        public class Baker : Baker<UnitMoverAuthoring>
        {
            public override void Bake(UnitMoverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover
                {
                    moveSpeed = authoring.moveSpeedMPS,
                    rotationSpeed = authoring.rotationSpeedRPS
                });
            }
        }
    }


    public struct UnitMover : IComponentData
    {
        /// <summary> Unit movement speed, in meters per second. </summary>
        public float moveSpeed;
        /// <summary> Unit rotation speed, in radians per second. </summary>
        public float rotationSpeed;
        /// <summary> Target position for the unit to move towards. </summary>
        public float3 targetPosition;

        public bool isMoving;
    }
}
