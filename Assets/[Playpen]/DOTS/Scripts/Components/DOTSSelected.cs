using Unity.Entities;
using UnityEngine;

/// <summary>
/// Selected DOTS component authoring MonoBehaviour.
/// </summary>
public class DOTSSelected : MonoBehaviour
{
    /// <summary> Visual visible when selected. </summary>
    [SerializeField]
    private GameObject visualGameObject;

    /// <summary> Scale for the visual representation of selection when selected. </summary>
    [SerializeField]
    private float showScale = 2.0f; // Scale for the visual representation of selection.

    
    /// <summary>
    /// ECS Baker class to convert the MonoBehaviour to an Entity with SelectedDOTS component.
    /// </summary>
    class SelectedBaker : Baker<DOTSSelected>
    {
        /// <summary>
        /// Converts the MonoBehaviour properties to an Entity with SelectedDOTS component.
        /// </summary>
        public override void Bake(DOTSSelected authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected
            {
                visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                showScale = authoring.showScale
            });
            SetComponentEnabled<Selected>(entity,false);
        }
    }
}


/// <summary>
/// DOTS component containing selection data.
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

