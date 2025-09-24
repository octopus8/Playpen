using UnityEngine;

namespace RTS
{
    
    [CreateAssetMenu()]
    public class BuildingsScriptableObject : ScriptableObject
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
