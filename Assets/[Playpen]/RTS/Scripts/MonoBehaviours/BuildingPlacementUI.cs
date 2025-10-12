using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    /// <summary>
    /// Controls the UI for building placement in the RTS game.
    /// Displays buttons for each building type and highlights the selected building.
    /// </summary>
    public class BuildingPlacementUI : MonoBehaviour
    {
        /// <summary> Container for building buttons. </summary>
        [Tooltip("Container for building buttons.")]
        [SerializeField] private RectTransform buildingContainer;
        
        /// <summary> Template for individual building button. </summary>
        [Tooltip("Template for individual building button.")]
        [SerializeField] private RectTransform buildingTemplate;
        
        /// <summary> Scriptable object containing the set of buildings. </summary>
        [Tooltip("Scriptable object containing the set of buildings.")]
        [SerializeField] private BuildingSetScriptableObject buildingSetScriptableObject;

        /// <summary> Dictionary mapping buildings to their corresponding UI buttons. </summary>
        private Dictionary<BuildingScriptableObject, BuildingPlacementUIButton> buttonSingleDictionary;
 
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Initializes the building placement UI by creating buttons for each building in the building set.
        /// </summary>
        private void Awake()
        {
            // Hide the template building button.
            buildingTemplate.gameObject.SetActive(false);

            // Iterate through each building in the building set and create a button for it.
            buttonSingleDictionary = new Dictionary<BuildingScriptableObject, BuildingPlacementUIButton>();
            foreach (BuildingScriptableObject building in buildingSetScriptableObject.buildings)
            {
                // Skip buildings that should not be shown in the placement UI.
                if (!building.showInBuildingPlacementUI)
                {
                    continue;
                }
                
                // Instantiate a new building button from the template.
                RectTransform buildingRectTransform = Instantiate(buildingTemplate, buildingContainer);
                buildingRectTransform.gameObject.SetActive(true);
                
                // Store the button in the dictionary.
                BuildingPlacementUIButton button = buildingRectTransform.GetComponent<BuildingPlacementUIButton>();
                buttonSingleDictionary[building] = button;

                // Setup the button with the building data.
                button.Setup(building);
            }
        }

        
        /// <summary>
        /// Start is called before the first frame update.
        /// Subscribes to building placement events and initializes the selected visual state.
        /// </summary>
        private void Start()
        {
            BuildingPlacement.Instance.OnActiveBuildingChanged += BuildingPlacement_OnActiveBuildingChanged;
            UpdateSelectedVisual();
        }

        
        /// <summary>
        /// OnDestroy is called when the MonoBehaviour will be destroyed.
        /// Unsubscribes from building placement events.
        /// </summary>
        private void OnDestroy()
        {
            BuildingPlacement.Instance.OnActiveBuildingChanged -= BuildingPlacement_OnActiveBuildingChanged;
        }


        /// <summary>
        /// Callback for when the active building changes in the BuildingPlacement system.
        /// Updates the UI to reflect the newly selected building.
        /// </summary>
        private void BuildingPlacement_OnActiveBuildingChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        
        /// <summary>
        /// Updates the visual state of the building buttons to indicate which building is currently selected.
        /// Hides the selected state for all buttons and shows it only for the active building's button.
        /// </summary>
        private void UpdateSelectedVisual()
        {
            foreach (BuildingScriptableObject building in buttonSingleDictionary.Keys)
            {
                buttonSingleDictionary[building].HideSelected();
            }

            buttonSingleDictionary[BuildingPlacement.Instance.GetActiveBuilding()].ShowSelected();
        }
    }
}
