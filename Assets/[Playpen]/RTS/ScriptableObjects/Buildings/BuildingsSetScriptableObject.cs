using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu()]
    public class BuildingsSetScriptableObject : ScriptableObject
    {
    
        [SerializeField]
        private List<BuildingsScriptableObject> buildings;
        
        public BuildingsScriptableObject GetBuilding(BuildingsScriptableObject.BuildingType buildingType)
        {
            return buildings.Find(building => building.buildingType == buildingType);
        }
    }
    
}
