using System;
using UnityEngine;


namespace RTS
{
    
    public class ResetPositionUI : MonoBehaviour
    {
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


