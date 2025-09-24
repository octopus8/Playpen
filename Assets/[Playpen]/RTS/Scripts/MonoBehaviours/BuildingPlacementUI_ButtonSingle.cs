using UnityEngine;
using UnityEngine.UI;

namespace RTS
{
    public class BuildingPlacementUI_ButtonSingle : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image selectedImage;
        
        private BuildingScriptableObject buildingTemplate;

        
        public void Setup(BuildingScriptableObject building)
        {
            buildingTemplate = building;
            
            GetComponent<Button>().onClick.AddListener(() =>
            {
                BuildingPlacement.Instance.SetActiveBuilding(buildingTemplate);
            });
            
            icon.sprite = building.sprite;
        }

        public void ShowSelected()
        {
            selectedImage.enabled = true;
        }
        public void HideSelected()
        {
            selectedImage.enabled = false;
        }
    }
    
}

