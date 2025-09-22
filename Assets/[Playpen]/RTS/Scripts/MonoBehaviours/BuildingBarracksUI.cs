using System;
using RTS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarracksUI : MonoBehaviour
{
    [SerializeField]
    private Button _createSoldierButton;
    
    [SerializeField]
    private Image _progressBarImage;

    [SerializeField] private RectTransform _unitQueueContainer;
    [SerializeField] private RectTransform _unitQueueItemTemplate;
    
    private Entity _buildingBarracksEntity;
    
    private EntityManager _entityManager;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Hide();
        _createSoldierButton.onClick.AddListener(() =>
        {
            DynamicBuffer<SpawnBuffer> spawnBuffer = _entityManager.GetBuffer<SpawnBuffer>(_buildingBarracksEntity, false);
            spawnBuffer.Add(new SpawnBuffer
            {
                UnitTypeID = UnitScriptableObject.UnitTypeID.Soldier
            });

        });
        UnitSelection.Instance.OnSelectedChanged += OnSelectedChanged;
        DOTSEvents.Instance.OnBarracksQueueChanged += OnBarracksQueueChanged;
        
        _unitQueueItemTemplate.gameObject.SetActive(false);
    }

    private void OnBarracksQueueChanged(object sender, EventArgs e)
    {
        Entity entity = (Entity)sender;
        if (entity == _buildingBarracksEntity)
        {
            UpdateUnitQueueVisual();
        }
    }


    private void Update()
    {
        UpdateProgressBarVisual();
    }

    private void OnSelectedChanged(object sender, EventArgs e)
    {
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, BuildingBarracks>().Build(_entityManager);
        
        NativeArray<Entity> selectedBarracks = entityQuery.ToEntityArray(Allocator.Temp);
        if (selectedBarracks.Length > 0)
        {
            _buildingBarracksEntity = selectedBarracks[0];
            Show();
            UpdateProgressBarVisual();
            UpdateUnitQueueVisual();
        }
        else
        {
            _buildingBarracksEntity = Entity.Null;
            Hide();
        }
        selectedBarracks.Dispose();
        entityQuery.Dispose();
    }

    private void UpdateProgressBarVisual()
    {
        if (_buildingBarracksEntity == Entity.Null)
        {
            _progressBarImage.fillAmount = 0;
            return;
        }

        BuildingBarracks barracks = _entityManager.GetComponentData<BuildingBarracks>(_buildingBarracksEntity);
        if (barracks.unitTypeID == UnitScriptableObject.UnitTypeID.None)
        {
            _progressBarImage.fillAmount = 0;
        }
        else
        {
            _progressBarImage.fillAmount = barracks.timer / barracks.spawnDuration;
        }

    }
    
    
    private void UpdateUnitQueueVisual()
    {
        Debug.Log("Updating Unit Queue Visual");
        foreach (Transform child in _unitQueueContainer)
        {
            if (child == _unitQueueItemTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        DynamicBuffer<SpawnBuffer> spawnBuffer = _entityManager.GetBuffer<SpawnBuffer>(_buildingBarracksEntity, true);
        foreach (var spawn in spawnBuffer)
        {
            RectTransform item = Instantiate(_unitQueueItemTemplate, _unitQueueContainer);
            item.gameObject.SetActive(true);
            // Here you can set the visual representation of the unit type
            // For example, if you have an Image component in the template:
            // Image unitImage = item.GetComponent<Image>();
            // unitImage.sprite = GetSpriteForUnitType(spawn.UnitTypeID);
        }
        
    }
    

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
