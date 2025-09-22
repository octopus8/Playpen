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

    private void OnSelectedChanged(object sender, EventArgs e)
    {
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, BuildingBarracks>().Build(_entityManager);
        
        NativeArray<Entity> selectedBarracks = entityQuery.ToEntityArray(Allocator.Temp);
        if (selectedBarracks.Length > 0)
        {
            _buildingBarracksEntity = selectedBarracks[0];
            Show();
        }
        else
        {
            _buildingBarracksEntity = Entity.Null;
            Hide();
        }
        selectedBarracks.Dispose();
        entityQuery.Dispose();
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
