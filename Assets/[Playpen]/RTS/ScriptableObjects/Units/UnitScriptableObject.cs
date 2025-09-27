using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RTS
{

    [CreateAssetMenu()]
    public class UnitScriptableObject : ScriptableObject
    {

        public UnitType unitType;
        
        public float spawnDuration = 2f;

        public Sprite unitIcon;
    
        public enum UnitType
        {
            None,
            Soldier,
            Scout,
            Zombie
        }


        public Entity GetUnit(EntityReferences entityReferences)
        {
            switch (unitType)
            {
                default:
                case UnitType.Soldier:
                    return entityReferences.soldierEntityPrefab;
                case UnitType.Scout:
                    return entityReferences.scoutEntityPrefab;
                case UnitType.Zombie:
                    return entityReferences.zombieEntityPrefab;
            }
        }
    
    }
    
    
}
