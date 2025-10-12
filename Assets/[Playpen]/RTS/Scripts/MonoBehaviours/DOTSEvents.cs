using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Singleton class that manages and triggers global events related to DOTS entities.
    /// Provides events for barracks queue changes and HQ death, allowing other systems to subscribe and respond.
    /// </summary>
    public class DOTSEvents : MonoBehaviour
    {
        /// <summary> Singleton instance of the DOTSEvents class. </summary>
        public static DOTSEvents Instance { get; private set; }

        /// <summary> Event handlers for barracks queue changes </summary>
        public event EventHandler OnBarracksQueueChanged;
    
        /// <summary> Event handler for HQ death. </summary>
        public event EventHandler OnHQDead;

    
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Initializes the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

    
        /// <summary>
        /// Called by the EventResetSystem when the barracks queue changes.
        /// Triggers the OnBarracksQueueChanged event for each affected entity.
        /// </summary>
        public void TriggerOnBarracksQueueChanged(NativeList<Entity> onUnitQueueChangedEntities)
        {
            foreach (var entity in onUnitQueueChangedEntities)
            {
                OnBarracksQueueChanged?.Invoke(entity, System.EventArgs.Empty);
            }
        }
    

        /// <summary>
        /// Called by the EventResetSystem when the friendly HQ is dead to trigger the OnHQDead event.
        /// </summary>
        public void TriggerOnHQDead()
        {
            OnHQDead?.Invoke(this, EventArgs.Empty);
        }
    }    
}
