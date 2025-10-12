using System;
using RTS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace RTS
{
    /// <summary>
    /// Controls the UI for the barracks building, allowing players to create units and view the spawn queue.
    /// The UI includes buttons for creating soldiers and scouts, a progress bar for spawn progress,
    /// and a visual representation of the unit queue.
    /// </summary>
    public class BuildingBarracksUI : MonoBehaviour
    {
        /// <summary> Create soldier button. </summary>
        [Tooltip("Create soldier button.")]
        [SerializeField] private Button _createSoldierButton;
        
        /// <summary> Create scout button. </summary>
        [Tooltip("Create scout button.")]
        [SerializeField] private Button _createScoutButton;
        
        /// <summary> Progress bar image. </summary>
        [Tooltip("Progress bar image.")]
        [SerializeField] private Image _progressBarImage;

        /// <summary> Unit queue container. </summary>
        [Tooltip("Unit queue container.")]
        [SerializeField] private RectTransform _unitQueueContainer;
        
        /// <summary> Unit queue item template. </summary>
        [Tooltip("Unit queue item template.")]
        [SerializeField] private RectTransform _unitQueueItemTemplate;

        /// <summary> The currently selected barracks entity. </summary>
        private Entity currentSelectedBarracksEntity;
        
        /// <summary> The entity manager. </summary>
        private EntityManager _entityManager;

        
        /// <summary>
        /// Start is called before the first frame update.
        /// Initially, the UI is hidden. Button listeners are added to enqueue unit spawn requests.
        /// The UI subscribes to selection change events and barracks queue change events to update its display.
        /// </summary>
        private void Start()
        {
            // Get the default entity manager.
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // Initially hide the UI.
            Hide();
            
            // Add Soldier button listener.
            _createSoldierButton.onClick.AddListener(() =>
            {
                // Enqueue a soldier unit spawn request by setting the spawn unit type and enabling the component.
                _entityManager.SetComponentData(currentSelectedBarracksEntity, new BuildingBarracksUnitEnqueue
                {
                    UnitType = UnitScriptableObject.UnitType.Soldier
                });
                _entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(currentSelectedBarracksEntity, true);
            });
            
            // Add Scout button listener.
            _createScoutButton.onClick.AddListener(() =>
            {
                // Enqueue a scout unit spawn request by setting the spawn unit type and enabling the component.
                _entityManager.SetComponentData(currentSelectedBarracksEntity, new BuildingBarracksUnitEnqueue
                {
                    UnitType = UnitScriptableObject.UnitType.Scout
                });
                _entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(currentSelectedBarracksEntity, true);
            });
            
            // Subscribe to selection change events.
            // This event is triggered when the player selects or deselects entities.
            UnitSelection.Instance.OnSelectedChanged += OnSelectedChanged;
            
            // Subscribe to barracks queue change events.
            // This event is triggered when the unit queue in the barracks changes (e.g., unit added or removed).
            DOTSEvents.Instance.OnBarracksQueueChanged += OnBarracksQueueChanged;
            
            // Hide the unit queue item template.
            _unitQueueItemTemplate.gameObject.SetActive(false);
        }


        /// <summary>
        /// OnDestroy is called when the MonoBehaviour will be destroyed.
        /// Unsubscribes from events.
        /// </summary>
        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks.
            if (UnitSelection.Instance != null)
            {
                UnitSelection.Instance.OnSelectedChanged -= OnSelectedChanged;
            }
            if (DOTSEvents.Instance != null)
            {
                DOTSEvents.Instance.OnBarracksQueueChanged -= OnBarracksQueueChanged;
            }
        }


        /// <summary>
        /// Update is called once per frame.
        /// Updates the progress bar visual to reflect the current spawn progress.
        /// </summary>
        private void Update()
        {
            UpdateProgressBarVisual();
        }


        /// <summary>
        /// Shows the barracks UI by activating the GameObject.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        
        /// <summary>
        /// Hides the barracks UI by deactivating the GameObject.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        
        /// <summary>
        /// Updates the progress bar visual based on the current spawn progress of the selected barracks.
        /// If no barracks is selected or no unit is being spawned, the progress bar is reset.
        /// </summary>
        private void UpdateProgressBarVisual()
        {
            // If no barracks is selected, reset the progress bar.
            if (currentSelectedBarracksEntity == Entity.Null)
            {
                _progressBarImage.fillAmount = 0;
                return;
            }

            // Get the BuildingBarracks component data to access spawn progress information.
            BuildingBarracks barracks = _entityManager.GetComponentData<BuildingBarracks>(currentSelectedBarracksEntity);
            
            // If no unit is being spawned, reset the progress bar.
            if (barracks.spawnType == UnitScriptableObject.UnitType.None)
            {
                _progressBarImage.fillAmount = 0;
            }
            
            // Otherwise, update the progress bar based on the spawn timer and duration.
            else
            {
                _progressBarImage.fillAmount = barracks.timer / barracks.currentSpawnDuration;
            }

        }

        
        /// <summary>
        /// Callback for when the selection changes.
        /// Queries for selected barracks and updates the UI accordingly.
        /// </summary>
        private void OnSelectedChanged(object sender, EventArgs e)
        {
            // Query for entities that are both selected and are barracks.
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitSelected, BuildingBarracks>().Build(_entityManager);
            NativeArray<Entity> selectedBarracks = entityQuery.ToEntityArray(Allocator.Temp);
            
            // If there is at least one selected barracks, set it as the current selected barracks and show the UI.
            if (selectedBarracks.Length > 0)
            {
                currentSelectedBarracksEntity = selectedBarracks[0];
                Show();
                UpdateProgressBarVisual();
                UpdateUnitQueueVisual();
            }
            
            // Otherwise, clear the current selected barracks and hide the UI.
            else
            {
                currentSelectedBarracksEntity = Entity.Null;
                Hide();
            }
            
            // Dispose of temporary allocations.
            selectedBarracks.Dispose();
            entityQuery.Dispose();
        }
        
        
        /// <summary>
        /// Callback for when the barracks queue changes.
        /// If the changed entity is the currently selected barracks, update the unit queue visual.
        /// </summary>
        private void OnBarracksQueueChanged(object sender, EventArgs e)
        {
            Entity entity = (Entity)sender;
            if (entity == currentSelectedBarracksEntity)
            {
                UpdateUnitQueueVisual();
            }
        }


        /// <summary>
        /// Updates the unit queue visual by clearing existing items and instantiating new ones based on
        /// the current spawn buffer of the selected barracks. Each unit in the queue is represented by an icon.
        /// </summary>
        private void UpdateUnitQueueVisual()
        {
            // Iterate through existing queue items and destroy them, except for the template.
            foreach (Transform child in _unitQueueContainer)
            {
                if (child == _unitQueueItemTemplate)
                {
                    continue;
                }
                Destroy(child.gameObject);
            }
            
            // Iterate through the spawn buffer and create a new UI item for each unit in the queue.
            DynamicBuffer<SpawnBuffer> spawnBuffer = _entityManager.GetBuffer<SpawnBuffer>(currentSelectedBarracksEntity, true);
            foreach (var spawn in spawnBuffer)
            {
                RectTransform item = Instantiate(_unitQueueItemTemplate, _unitQueueContainer);
                item.gameObject.SetActive(true);
                UnitScriptableObject unitType = RTSGame.Instance.units.GetUnit(spawn.unitType);
                item.GetComponent<Image>().sprite = unitType.unitIcon;
            }
        }
    }
}

