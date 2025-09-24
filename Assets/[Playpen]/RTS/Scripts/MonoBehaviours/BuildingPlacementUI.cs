using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{

    
    public class BuildingPlacementUI : MonoBehaviour
    {
        [SerializeField] private RectTransform buildingContainer;
        [SerializeField] private RectTransform buildingTemplate;
        [SerializeField] private BuildingSetScriptableObject buildingSetScriptableObject;

        private Dictionary<BuildingScriptableObject, BuildingPlacementUI_ButtonSingle> buttonSingleDictionary;
 
        private void Awake()
        {
            buildingTemplate.gameObject.SetActive(false);
            buttonSingleDictionary = new Dictionary<BuildingScriptableObject, BuildingPlacementUI_ButtonSingle>();
            foreach (BuildingScriptableObject building in buildingSetScriptableObject.buildings)
            {
                if (!building.showInBuildingPlacementUI)
                {
                    continue;
                }
                RectTransform buildingRectTransform = Instantiate(buildingTemplate, buildingContainer);
                buildingRectTransform.gameObject.SetActive(true);
                BuildingPlacementUI_ButtonSingle buttonSingle = buildingRectTransform.GetComponent<BuildingPlacementUI_ButtonSingle>();
                buttonSingleDictionary[building] = buttonSingle;
                buttonSingle.Setup(building);
                
                
//                buildingRectTransform.GetComponent<BuildingDragHandler>().SetBuilding(building);
//                 buildingRectTransform.GetComponent<UnityEngine.UI.Image>().sprite = building.sprite;
            }
        }

        private void Start()
        {
            BuildingPlacement.Instance.OnActiveBuildingChanged += BuildingPlacement_OnActiveBuildingChanged;
            UpdateSelectedVisual();
        }

        private void BuildingPlacement_OnActiveBuildingChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

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
