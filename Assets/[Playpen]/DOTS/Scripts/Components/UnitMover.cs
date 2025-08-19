using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    
    [SerializeField]
    private float rotationSpeed = 10f;
    
    public class Baker : Baker<UnitMover>
    {
        public override void Bake(UnitMover authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMoverDOTS
            {
                moveSpeed = authoring.moveSpeed,
                rotationSpeed = authoring.rotationSpeed
            });
        }
    }
}

public struct UnitMoverDOTS : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float3 targetPosition;
}
