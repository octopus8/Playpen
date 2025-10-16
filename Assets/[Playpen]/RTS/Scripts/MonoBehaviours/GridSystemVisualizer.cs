using Unity.Entities;
using UnityEngine;


namespace RTS
{
    public class GridSystemVisualizer : MonoBehaviour
    {
        public static GridSystemVisualizer Instance { get; private set; }
        
        [SerializeField] private Transform visualPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private bool isInited = false;

        public void InitializeGrid(GridSystem.GridSystemData data)
        {
            if (isInited)
            {
                return;
            }
            isInited = true;
            for (int x = 0; x < data.width; x++)
            {
                for (int y = 0; y < data.height; y++)
                {
                    Transform visual = Instantiate(visualPrefab);
                    visual.GetComponent<GridSystemVisual>().Setup(x, y, data.gridNodeSize);
                }
            }
        }

    }
    
}

