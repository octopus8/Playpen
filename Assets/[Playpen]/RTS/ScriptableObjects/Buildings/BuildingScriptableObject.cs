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
            ZombieBuilding,
            FriendlyTower,
            FriendlyBarracks
        }

        public bool IsNone()
        {
            return buildingType == BuildingType.None;
        }

        public Entity GetBuilding(EntityReferences entityReferences)
        {
            switch (buildingType)
            {
                default:
                case BuildingType.ZombieBuilding:
                case BuildingType.FriendlyTower:
                    return entityReferences.buildingTowerPrefab;
                case BuildingType.FriendlyBarracks:
                    return entityReferences.buildingBarracksPrefab;
            }
        }

    }
    
}
