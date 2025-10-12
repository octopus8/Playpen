using Unity.Entities;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// A scriptable object that holds data for a building.
    /// </summary>
    [CreateAssetMenu()]
    public class BuildingScriptableObject : ScriptableObject
    {
        /// <summary> The type of building. </summary>
        [Tooltip("The type of building.")]
        public BuildingType buildingType;

        /// <summary> The prefab of the building. </summary>
        [Tooltip("The prefab of the building.")]
        public Transform prefab;

        /// <summary> The minimum distance required between this building and other buildings when placing it. </summary>
        [Tooltip("The minimum distance required between this building and other buildings when placing it.")]
        public float buildingDistanceMin = 10;
        
        /// <summary> Whether to show this building in the building placement UI. </summary>
        [Tooltip("Whether to show this building in the building placement UI.")]
        public bool showInBuildingPlacementUI = true;
        
        /// <summary> The sprite to use for this building in the building placement UI. </summary>
        [Tooltip("The sprite to use for this building in the building placement UI.")]
        public Sprite sprite;
        
        /// <summary> The ghost prefab to use when placing this building. </summary>
        [Tooltip("The ghost prefab to use when placing this building.")]
        public Transform ghostPrefab;
        
        
        /// <summary> Building types. </summary>
        public enum BuildingType
        {
            None,
            EnemyBuilding,
            FriendlyTower,
            FriendlyBarracks,
            FriendlyHQ
        }

        
        /// <summary>
        /// Determines if this building type is 'None'.
        /// </summary>
        public bool IsNone()
        {
            return buildingType == BuildingType.None;
        }

        
        /// <summary>
        /// Gets the corresponding building entity prefab from the given EntityPrefabSet based on the building type.
        /// </summary>
        public Entity GetBuilding(EntityPrefabSet entityPrefabSet)
        {
            switch (buildingType)
            {
                default:
                case BuildingType.EnemyBuilding:
                case BuildingType.FriendlyTower:
                    return entityPrefabSet.buildingTowerPrefab;
                case BuildingType.FriendlyBarracks:
                    return entityPrefabSet.buildingBarracksPrefab;
            }
        }
    }
}
