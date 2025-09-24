using UnityEngine;

namespace RTS
{
    
    [CreateAssetMenu()]
    public class BuildingScriptableObject : ScriptableObject
    {
        public BuildingType buildingType;

        public Transform prefab;
        
        
        public enum BuildingType
        {
            None,
            ZombieBuilding,
            FriendlyTower,
            FriendlyBarracks
        }
    }
    
}
