using System;
using RTS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class BarracksUI : MonoBehaviour
{
    [SerializeField]
    private Button _createSoldierButton;
    
    [SerializeField]
    private Image _progressBarImage;
    
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
    

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
