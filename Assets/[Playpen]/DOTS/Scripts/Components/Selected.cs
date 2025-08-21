using Unity.Entities;
using UnityEngine;

/// <summary>
/// SelectedDOTS ECS component authoring MonoBehaviour.
/// </summary>
public class Selected : MonoBehaviour
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
    class SelectedBaker : Baker<Selected>
    {
        /// <summary>
        /// Converts the MonoBehaviour properties to an Entity with SelectedDOTS component.
        /// </summary>
        public override void Bake(Selected authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SelectedDOTS
            {
                visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                showScale = authoring.showScale
            });
            SetComponentEnabled<SelectedDOTS>(entity,false);
        }
    }
}


/// <summary>
/// DOTS component containing selection data.
/// </summary>
public struct SelectedDOTS : IComponentData, IEnableableComponent
{
    /// <summary> Entity representing the visual that is shown when selected. </summary>
    public Entity visualEntity;
    /// <summary> Scale for the visual representation of selection when selected. </summary>
    public float showScale;
}

