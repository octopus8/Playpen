using Unity.Entities;
using UnityEngine;


namespace RTS
{

    /// <summary>
    /// Authoring component for selection data. This component is added to all selectable entities.
    /// </summary>
    public class SelectedAuthoring : MonoBehaviour
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
        class SelectedBaker : Baker<SelectedAuthoring>
        {
            /// <summary>
            /// Converts the MonoBehaviour properties to an Entity with SelectedDOTS component.
            /// </summary>
            public override void Bake(SelectedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selected
                {
                    visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                    showScale = authoring.showScale
                });
                SetComponentEnabled<Selected>(entity, false);
            }
        }
    }


    /// <summary>
    /// Enableable component storing selection data for an entity.
    /// </summary>
    public struct Selected : IComponentData, IEnableableComponent
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
