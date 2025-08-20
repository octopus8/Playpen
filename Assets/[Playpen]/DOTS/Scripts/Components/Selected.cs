using Unity.Entities;
using UnityEngine;

public class Selected : MonoBehaviour
{
    [SerializeField]
    private GameObject visualGameObject;

    [SerializeField]
    private float showScale = 2.0f; // Scale for the visual representation of selection.
    
    class SelectedBaker : Baker<Selected>
    {
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

public struct SelectedDOTS : IComponentData, IEnableableComponent
{
    public Entity visualEntity; // Reference to the visual entity for this selection
    public float showScale;
}

