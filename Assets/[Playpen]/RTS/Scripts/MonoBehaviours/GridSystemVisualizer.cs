using Unity.Entities;
using UnityEngine;


namespace RTS
{
    public class GridSystemVisualizer : MonoBehaviour
    {
        public static GridSystemVisualizer Instance { get; private set; }
        
        [SerializeField] private Transform visualPrefab;
        
        private bool isInited = false;
        private GridSystemVisual[,] gridVisuals;


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


        public void InitializeGrid(GridSystem.GridSystemData data)
        {
            if (isInited)
            {
                return;
            }
            isInited = true;
            gridVisuals = new GridSystemVisual[data.width, data.height];
            for (int x = 0; x < data.width; x++)
            {
                for (int y = 0; y < data.height; y++)
                {
                    Transform visual = Instantiate(visualPrefab);
                    GridSystemVisual gridSystemVisual = visual.GetComponent<GridSystemVisual>();
                    gridSystemVisual.Setup(x, y, data.gridNodeSize);
                    
                    gridVisuals[x, y] = gridSystemVisual;
                }
            }
        }

        public void UpdateGrid(GridSystem.GridSystemData data)
        {
            for (int x = 0; x < data.width; x++)
            {
                for (int y = 0; y < data.height; y++)
                {
                    GridSystemVisual gridSystemVisual = gridVisuals[x, y];
                    
                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    int index = GridSystem.CalculateIndex(x, y, data.width);
                    Entity gridNodeEntity = data.gridMap.gridEntityArray[index];
                    GridSystem.GridNode gridNode = entityManager.GetComponentData<GridSystem.GridNode>(gridNodeEntity);
                    
                    gridSystemVisual.SetColor(gridNode.data == 0 ? Color.white : Color.red);
                }
            }
            
        }

    }
    
}

