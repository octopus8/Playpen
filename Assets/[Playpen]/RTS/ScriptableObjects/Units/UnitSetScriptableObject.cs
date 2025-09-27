using System.Collections.Generic;
using UnityEngine;



namespace RTS
{
    [CreateAssetMenu()]
    public class UnitSetScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<UnitScriptableObject> units;
        
        public UnitScriptableObject GetUnit(UnitScriptableObject.UnitType unitType)
        {
            return units.Find(unit => unit.unitType == unitType);
        }
    }
    
}

