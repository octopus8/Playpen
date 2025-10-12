using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// A scriptable object that holds a set of buildings.
    /// </summary>
    [CreateAssetMenu()]
    public class BuildingSetScriptableObject : ScriptableObject
    {
        /// <summary> The list of buildings. </summary>
        [Tooltip("The list of buildings.")]
        public List<BuildingScriptableObject> buildings;

        /// <summary> A reference to a 'none' building. </summary>
        [Tooltip("A reference to a 'none' building.")]
        public BuildingScriptableObject none;
        
        
        /// <summary>
        /// Gets the BuildingScriptableObject corresponding to the given building type.
        /// Returns null if no matching building is found.
        /// </summary>
        public BuildingScriptableObject GetBuilding(BuildingScriptableObject.BuildingType buildingType)
        {
            return buildings.Find(building => building.buildingType == buildingType);
        }
    }
    
}
