using UnityEngine;
using UnityEngine.UI;

namespace RTS
{
    /// <summary>
    /// Represents a UI button for selecting a building for placement.
    /// Controls the button's selection image visibility based on whether it is the currently selected building.
    /// </summary>
    public class BuildingPlacementUIButton : MonoBehaviour
    {
        /// <summary> Icon image for the building. </summary>
        [Tooltip("Icon image for the building.")]
        [SerializeField] private Image icon;
        
        /// <summary> Image to indicate selection state. </summary>
        [Tooltip("Image to indicate selection state.")]
        [SerializeField] private Image selectedImage;

        /// <summary> The building template associated with this button. </summary>
        private BuildingScriptableObject buildingTemplate;

        
        /// <summary>
        /// Sets up the button with the given building template.
        /// Configures the button's icon and click behavior.
        /// </summary>
        public void Setup(BuildingScriptableObject building)
        {
            // Store the building template.
            buildingTemplate = building;
            
            // Add a click listener to set the active building in the BuildingPlacement singleton.
            GetComponent<Button>().onClick.AddListener(() =>
            {
                BuildingPlacement.Instance.SetActiveBuilding(buildingTemplate);
            });
            
            // Set the icon image to the building's sprite.
            icon.sprite = building.sprite;
        }

        
        /// <summary>
        /// Shows the selection indicator on the button.
        /// </summary>
        public void ShowSelected()
        {
            selectedImage.enabled = true;
        }
        
        
        /// <summary>
        /// Hides the selection indicator on the button.
        /// </summary>
        public void HideSelected()
        {
            selectedImage.enabled = false;
        }
    }
}

