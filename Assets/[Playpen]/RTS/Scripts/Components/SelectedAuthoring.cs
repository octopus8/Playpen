using Unity.Entities;
using UnityEngine;


namespace RTS
{

    /// <summary>
    /// Selected ECS component authoring MonoBehaviour. This component is added to entities that can be selected.
    /// It includes a reference to a visual GameObject that is shown when the entity is selected.
    /// </summary>
    public class SelectedAuthoring : MonoBehaviour
    {
        /// <summary> Visual visible when selected. </summary>
        [Tooltip("Visual visible when selected.")]
        [SerializeField] private GameObject visualGameObject;

        /// <summary> Scale for the visual representation of selection when selected. </summary>
        [Tooltip("Scale for the visual representation of selection when selected.")]
        [SerializeField] private float showScale = 2.0f; // Scale for the visual representation of selection.


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
    /// DOTS component containing selection data. This component is enableable to allow toggling selection state.
    /// </summary>
    public struct Selected : IComponentData, IEnableableComponent
    {
        /// <summary> Entity representing the visual that is shown when selected. </summary>
        public Entity visualEntity;

        /// <summary> Scale for the visual representation of selection when selected. </summary>
        public float showScale;

        /// <summary> Flag indicating if the entity has just been selected. </summary>
        public bool onSelected;

        /// <summary> Flag indicating if the entity has just been deselected. </summary>
        public bool onDeselected;
    }

}
