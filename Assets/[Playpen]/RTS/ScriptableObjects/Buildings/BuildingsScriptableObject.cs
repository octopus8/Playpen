using UnityEngine;

namespace RTS
{
    
    [CreateAssetMenu()]
    public class BuildingsScriptableObject : ScriptableObject
    {
        public BuildingType buildingType;
        
        public enum BuildingType
        {
            None,
            ZombieSpawner,
            FriendlyTower,
            FriendlyBarracks
        }
    }
    
}
