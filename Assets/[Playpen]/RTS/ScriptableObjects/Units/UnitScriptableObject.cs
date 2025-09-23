using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RTS
{

    [CreateAssetMenu()]
    public class UnitScriptableObject : ScriptableObject
    {

        public UnitTypeID unitTypeID;
        
        public float spawnDuration = 2f;

        public Sprite unitIcon;
    
        public enum UnitTypeID
        {
            None,
            Soldier,
            Scout,
            Zombie
        }


        public Entity GetUnit(EntityReferences entityReferences)
        {
            switch (unitTypeID)
            {
                default:
                case UnitTypeID.Soldier:
                    return entityReferences.soldierEntityPrefab;
                case UnitTypeID.Scout:
                    return entityReferences.scoutEntityPrefab;
                case UnitTypeID.Zombie:
                    return entityReferences.zombieEntityPrefab;
            }
        }
    
    }
    
    
}
