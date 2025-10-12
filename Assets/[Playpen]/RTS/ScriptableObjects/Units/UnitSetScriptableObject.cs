using System.Collections.Generic;
using UnityEngine;



namespace RTS
{
    /// <summary>
    /// A scriptable object that holds the set of units.
    /// </summary>
    [CreateAssetMenu()]
    public class UnitSetScriptableObject : ScriptableObject
    {
        /// <summary> The list of units. </summary>
        [SerializeField] private List<UnitScriptableObject> units;

        
        /// <summary>
        /// Gets the UnitScriptableObject corresponding to the given unit type.
        /// Returns null if no matching unit is found.
        /// </summary>
        public UnitScriptableObject GetUnit(UnitScriptableObject.UnitType unitType)
        {
            return units.Find(unit => unit.unitType == unitType);
        }
    }
    
}

