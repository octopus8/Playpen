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


        public Entity GetUnit(EntityPrefabSet entityPrefabSet)
        {
            switch (unitType)
            {
                default:
                case UnitType.Soldier:
                    return entityPrefabSet.soldierEntityPrefab;
                case UnitType.Scout:
                    return entityPrefabSet.scoutEntityPrefab;
                case UnitType.Zombie:
                    return entityPrefabSet.zombieEntityPrefab;
            }
        }
    
    }
    
    
}
