using Unity.Entities;
using UnityEngine;

public class MoveSpeed : MonoBehaviour
{
    [SerializeField]
    private float value = 50f;
    
    public class Baker : Baker<MoveSpeed>
    {
        public override void Bake(MoveSpeed authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveSpeedDOTS { value = authoring.value });
        }
    }
}

public struct MoveSpeedDOTS : IComponentData
{
    public float value;
}
