using System.Collections.Generic;
using UnityEngine;



namespace RTS
{
    [CreateAssetMenu()]
    public class UnitSetScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<UnitScriptableObject> units;
        
        public UnitScriptableObject GetUnit(UnitScriptableObject.UnitTypeID unitTypeID)
        {
            return units.Find(unit => unit.unitTypeID == unitTypeID);
        }
    }
    
}

