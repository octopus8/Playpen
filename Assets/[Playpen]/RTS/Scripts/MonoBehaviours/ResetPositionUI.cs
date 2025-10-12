using System;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Resets the RectTransform's anchored position and sizeDelta to zero on Awake.
    /// Then destroys this component to prevent further updates.
    /// </summary>
    /// <remarks>
    /// To make working with UI layers easier, during development, they can be placed anywhere in the Canvas.
    /// This component ensures that the UI element is reset to the center of its parent with no size offset.
    /// </remarks>
    public class ResetPositionUI : MonoBehaviour
    {
        /// <summary>
        /// On Awake, reset the RectTransform's anchored position and sizeDelta to zero.
        /// Then destroy this component to prevent further updates.
        /// </summary>
        private void Awake()
        {
            RectTransform  transform;
            transform = GetComponent<RectTransform>();
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
            Destroy(this);
        }
    }
    
    
    
}


