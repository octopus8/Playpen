using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DOTSEvents : MonoBehaviour
{
    public static DOTSEvents Instance { get; private set; }
    
    public event EventHandler OnBarracksQueueChanged;
    public event EventHandler OnHQDead;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    public void TriggerOnBarracksQueueChanged(NativeList<Entity> onUnitQueueChangedEntities)
    {
        foreach (var entity in onUnitQueueChangedEntities)
        {
            OnBarracksQueueChanged?.Invoke(entity, System.EventArgs.Empty);
        }
    }
    
    public void TriggerOnHQDead()
    {
        OnHQDead?.Invoke(this, EventArgs.Empty);
    }
    
}
