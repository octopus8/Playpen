using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu()]
    public class BuildingSetScriptableObject : ScriptableObject
    {
    
        [SerializeField]
        public List<BuildingScriptableObject> buildings;

        public BuildingScriptableObject none;
        
        public BuildingScriptableObject GetBuilding(BuildingScriptableObject.BuildingType buildingType)
        {
            return buildings.Find(building => building.buildingType == buildingType);
        }
    }
    
}
