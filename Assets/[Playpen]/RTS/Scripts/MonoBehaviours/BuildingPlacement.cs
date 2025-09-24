using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS
{
    public class BuildingPlacement : MonoBehaviour
    {
    
        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = MouseWorldPosition.Instance.GetMouseWorldPosition();

                
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
                EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntityReferences));
                EntityReferences entityReferences = entityQuery.GetSingleton<EntityReferences>();
                Entity entity = entityManager.Instantiate(entityReferences.buildingTowerPrefab);
                entityManager.SetComponentData(entity, LocalTransform.FromPosition(mousePosition));
            }
        }
    }
    
}
