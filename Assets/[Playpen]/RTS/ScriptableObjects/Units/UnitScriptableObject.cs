using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// A scriptable object that holds data for a unit.
    /// </summary>
    [CreateAssetMenu()]
    public class UnitScriptableObject : ScriptableObject
    {
        /// <summary> The type of unit. </summary>
        public UnitType unitType;
        
        /// <summary> Time it takes to spawn the unit in seconds. </summary>
        public float spawnDuration = 2f;
        
        /// <summary> The sprite to use for this unit in the UI. </summary>
        public Sprite unitIcon;
    
        
        /// <summary> Unit types. </summary>
        public enum UnitType
        {
            None,
            Soldier,
            Scout,
            Zombie
        }


        /// <summary>
        /// Gets the corresponding unit entity prefab from the given EntityPrefabSet based on the unit type.
        /// </summary>
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
