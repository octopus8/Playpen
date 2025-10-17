using RTS;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private int x;
    private int y;
    
    public void Setup(int x, int y, float gridNodeSize)
    {
        this.x = x;
        this.y = y;
        
        transform.position = GridSystem.GetWorldPosition(x, y, gridNodeSize);
        gameObject.name = $"GridNodeVisual {x},{y}";
        
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
    
}
