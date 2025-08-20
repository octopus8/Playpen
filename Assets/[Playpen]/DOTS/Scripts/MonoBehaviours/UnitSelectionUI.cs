using System;
using UnityEngine;

public class UnitSelectionUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectionArea;


    private void Start()
    {
        UnitSelection.Instance.OnSelectionAreaStart += OnSelectionAreaStart;
        UnitSelection.Instance.OnSelectionAreaEnd += OnSelectionAreaEnd;
        selectionArea.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (selectionArea.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }
    
    private void OnSelectionAreaStart(object sender, EventArgs e)
    {
        selectionArea.gameObject.SetActive(true);
        UpdateVisual();
    }
    
    private void OnSelectionAreaEnd(object sender, EventArgs e)
    {
        selectionArea.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelection.Instance.GetSelectionAreaRect();
        selectionArea.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y);
        selectionArea.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height);
    }
    
}
