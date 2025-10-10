using Unity.Entities;
using UnityEngine;


namespace RTS
{

    /// <summary>
    /// Authoring component for selection data. This component is added to all selectable entities.
    /// The component is added in a disabled state and is enabled when the entity is selected.
    /// </summary>
    public class UnitSelectedAuthoring : MonoBehaviour
    {
        /// <summary> Visual visible when selected. </summary>
        [Tooltip("Visual visible when selected.")]
        [SerializeField] private GameObject visualGameObject;

        /// <summary> Scale for the visual representation of selection when selected. </summary>
        [Tooltip("Scale for the visual representation of selection when selected.")]
        [SerializeField] private float showScale = 2.0f; // Scale for the visual representation of selection.


        /// <summary>
        /// Baker class for converting the authoring component to an ECS component.
        /// </summary>
        class UnitSelectedBaker : Baker<UnitSelectedAuthoring>
        {
            /// <summary>
            /// Adds the UnitSelected component to the entity with the specified parameters.
            /// The component is added in a disabled state.
            /// </summary>
            public override void Bake(UnitSelectedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitSelected
                {
                    visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                    showScale = authoring.showScale
                });
                SetComponentEnabled<UnitSelected>(entity, false);
            }
        }
    }


    /// <summary>
    /// Enableable component storing selection data for an entity.
    /// </summary>
    public struct UnitSelected : IComponentData, IEnableableComponent
    {
        /// <summary> Entity representing the visual that is shown when selected. </summary>
        public Entity visualEntity;

        /// <summary> Scale for the visual representation of selection when selected. </summary>
        public float showScale;

        /// <summary> Event flag indicating if the entity has just been selected. </summary>
        public bool onSelected;

        /// <summary> Event flag indicating if the entity has just been deselected. </summary>
        public bool onDeselected;
    }

}
