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

    private void Start()
    {
        Hide();
        _createSoldierButton.onClick.AddListener(() => 
        {
            
        });
        UnitSelection.Instance.OnSelectedChanged += OnSelectedChanged;
    }

    private void OnSelectedChanged(object sender, EventArgs e)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, BuildingBarracks>().Build(entityManager);
        
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
//        selectedBarracks.Dispose();
//        entityQuery.Dispose();
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
