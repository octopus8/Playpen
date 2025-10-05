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

        
        
        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        public class Baker : Baker<UnitMoverAuthoring>
        {
            public override void Bake(UnitMoverAuthoring authoring)
            {
                // Create an entity and add the UnitMover component with the specified parameters.
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover
                {
                    moveSpeed = authoring.moveSpeedMPS,
                    rotationSpeed = authoring.rotationSpeedRPS
                });
            }
        }
    }


    /// <summary>
    /// Component storing movement data for a unit.
    /// </summary>
    public struct UnitMover : IComponentData
    {
        /// <summary> Unit movement speed, in meters per second. </summary>
        public float moveSpeed;
        
        /// <summary> Unit rotation speed, in radians per second. </summary>
        public float rotationSpeed;
        
        /// <summary> Destination to move the unit to. </summary>
        public float3 destination;

        /// <summary> Flag indicating whether the unit is currently moving. </summary>
        public bool isMoving;
    }
}
