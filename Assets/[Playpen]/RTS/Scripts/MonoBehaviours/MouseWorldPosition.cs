using UnityEngine;


/// <summary>
/// MonoBehaviour to get the mouse world position in Unity.
/// This class provides methods to retrieve the mouse position in the world space,
/// specifically on a ground plane or any other defined plane.
/// It uses raycasting to determine where the mouse intersects with the ground plane.
/// </summary>
public class MouseWorldPosition : MonoBehaviour
{
    /// <summary> Singleton instance of MouseWorldPosition. </summary>
    public static MouseWorldPosition Instance { get; private set; }

    /// <summary>
    /// Stores the singleton instance of MouseWorldPosition.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }
    
    
    /// <summary>
    /// Gets the mouse world position by casting a ray from the camera to the mouse position.
    /// This method uses Physics.Raycast to determine where the ray intersects with the ground plane.
    /// </summary>
    /// <returns>World position of the mouse on the ground plane.</returns>
    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }

        return Vector3.zero; // Return zero if the ray does not hit the ground plane
    }
    

    /// <summary>
    /// Gets the mouse world position on a predefined plane (e.g., ground plane).
    /// This method uses a Plane object to determine where the ray intersects with the plane.
    /// </summary>
    /// <returns>World position of the mouse on the ground plane.</returns>
    public Vector3 GetMouseWorldPositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero; // Return zero if the ray does not hit the ground plane
    }
    
}
