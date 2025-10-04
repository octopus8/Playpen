using Unity.Entities;
using UnityEngine;

namespace RTS
{
    
    [CreateAssetMenu()]
    public class BuildingScriptableObject : ScriptableObject
    {
        public BuildingType buildingType;

        public Transform prefab;

        public float buildingDistanceMin = 10;
        
        public bool showInBuildingPlacementUI = true;
        
        public Sprite sprite;
        
        public Transform ghostPrefab;
        
        
        public enum BuildingType
        {
            None,
            EnemyBuilding,
            FriendlyTower,
            FriendlyBarracks,
            FriendlyHQ
        }

        public bool IsNone()
        {
            return buildingType == BuildingType.None;
        }

        public Entity GetBuilding(EntityPrefabSet entityPrefabSet)
        {
            switch (buildingType)
            {
                default:
                case BuildingType.EnemyBuilding:
                case BuildingType.FriendlyTower:
                    return entityPrefabSet.buildingTowerPrefab;
                case BuildingType.FriendlyBarracks:
                    return entityPrefabSet.buildingBarracksPrefab;
            }
        }

    }
    
}
