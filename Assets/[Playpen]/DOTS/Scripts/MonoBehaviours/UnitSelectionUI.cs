using System;
using UnityEngine;

/// <summary>
/// MonoBehaviour that handles the UI for unit selection in a game.
/// It listens for selection area start and end events, updates the visual representation of the selection area,
/// and manages the visibility of the selection area UI element.
/// </summary>
public class UnitSelectionUI : MonoBehaviour
{
    /// <summary> The selection area UI element that visually represents the area of selection. </summary>
    [SerializeField] private RectTransform selectionArea;


    /// <summary>
    /// Sets callbacks for selection area start and end events, and initializes the selection area UI element.
    /// </summary>
    private void Start()
    {
        UnitSelection.Instance.OnSelectionAreaStart += OnSelectionAreaStart;
        UnitSelection.Instance.OnSelectionAreaEnd += OnSelectionAreaEnd;
        selectionArea.gameObject.SetActive(false);
    }
    
    
    /// <summary>
    /// Updates the selection area visual representation if the selection area is active.
    /// </summary>
    private void Update()
    {
        if (selectionArea.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }
    
    
    /// <summary>
    /// Handles the start of the selection area by activating the selection area UI element
    /// and updating its visual representation.
    /// </summary>
    private void OnSelectionAreaStart(object sender, EventArgs e)
    {
        selectionArea.gameObject.SetActive(true);
        UpdateVisual();
    }
    
    
    /// <summary>
    /// Handles the end of the selection area by deactivating the selection area UI element.
    /// This method is called when the user releases the mouse button after starting a selection.
    /// </summary>
    private void OnSelectionAreaEnd(object sender, EventArgs e)
    {
        selectionArea.gameObject.SetActive(false);
    }

    
    /// <summary>
    /// Updates the visual representation of the selection area based on the current selection rectangle.
    /// This method retrieves the selection area rectangle from the UnitSelection singleton and updates the RectTransform
    /// of the selection area UI element to match the rectangle's position and size.
    /// </summary>
    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelection.Instance.GetSelectionAreaRect();
        selectionArea.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y);
        selectionArea.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height);
    }
    
}
