using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu()]
    public class BuildingsSetScriptableObject : ScriptableObject
    {
    
        [SerializeField]
        private List<BuildingScriptableObject> buildings;
        
        public BuildingScriptableObject GetBuilding(BuildingScriptableObject.BuildingType buildingType)
        {
            return buildings.Find(building => building.buildingType == buildingType);
        }
    }
    
}
