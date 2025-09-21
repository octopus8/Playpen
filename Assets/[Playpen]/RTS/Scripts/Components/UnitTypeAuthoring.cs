using Unity.Entities;
using UnityEngine;

namespace RTS
{

    public class UnitTypeAuthoring : MonoBehaviour
    {
        
        /// <summary>Type of unit.</summary>
        [Tooltip("Type of unit.")]
        [SerializeField]
        private UnitScriptableObject.UnitType unitType;

        public class Baker : Baker<UnitTypeAuthoring>
        {
            public override void Bake(UnitTypeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitType
                {
                    unitType = authoring.unitType,
                });
            }
        }
    }

    
    public struct UnitType : IComponentData
    {
        public UnitScriptableObject.UnitType unitType;
    }
    
}
