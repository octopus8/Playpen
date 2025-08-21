using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


/// <summary>
/// UnitMover ECS component authoring MonoBehaviour.
/// </summary>
public class DOTSUnitMover : MonoBehaviour
{
    /// <summary> Unit movement speed, in meters per second. </summary>
    [Tooltip("Unit movement speed.")]
    [SerializeField]
    private float moveSpeedMPS = 5f;

    /// <summary> Unit rotation speed, in radians per second. </summary>
    [Tooltip("Unit rotation speed.")]
    [SerializeField]
    private float rotationSpeedRPS = 10f;
    
    
    /// <summary>
    /// ECS Baker class to convert the MonoBehaviour to an Entity with UnitMoverDOTS component.
    /// </summary>
    public class Baker : Baker<DOTSUnitMover>
    {
        /// <summary>
        /// Converts the MonoBehaviour properties to an Entity with UnitMoverDOTS component.
        /// </summary>
        public override void Bake(DOTSUnitMover authoring)
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


/// <summary>
/// DOTS component containing unit movement data.
/// </summary>
public struct UnitMover : IComponentData
{
    /// <summary> Unit movement speed, in meters per second. </summary>
    public float moveSpeed;
    
    /// <summary> Unit rotation speed, in radians per second. </summary>
    public float rotationSpeed;
    
    /// <summary> Target position for the unit to move towards. </summary>
    public float3 targetPosition;
}
