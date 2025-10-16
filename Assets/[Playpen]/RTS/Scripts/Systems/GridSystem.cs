#define VISUALIZE_GRID

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace RTS
{

    /// <summary>
    /// A grid system that manages a grid of nodes.
    /// Each node is represented by an entity with a GridNode component.
    /// The grid system itself holds a GridSystemData component that contains the grid parameters and a reference to the grid map.
    /// </summary>
    /// <remarks>
    /// The GridSystem struct is public so that it can be accessed by other systems or components.
    /// </remarks>
    [BurstCompile]
    public partial struct GridSystem : ISystem
    {
        public struct GridSystemData : IComponentData
        {
            public int width;
            public int height;
            public float gridNodeSize;
            public GridMap gridMap;
        }

        public struct GridMap
        {
            public NativeArray<Entity> gridEntityArray;
        }
        
        public struct GridNode : IComponentData
        {
            public int x;
            public int y;
            public byte data;
        }
        
        
        /// <summary>
        /// Called when the system is created.
        /// Initializes the grid map and grid nodes.
        /// </summary>
        /// <remarks>
        /// Authoring components are often used to set up data for systems.
        /// In this case, we are directly creating and initializing the grid map and nodes in the system's OnCreate method.
        /// This approach is suitable for simple setups where the grid parameters are hardcoded.
        /// For more complex scenarios, consider using authoring components to allow for more flexible configuration.
        /// </remarks>
        /// <param name="state"></param>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            int width = 20;
            int height = 10;
            float gridNodeSize = 5f;
            int totalNodes = width * height;

            // Create a prefab entity for grid nodes.
            Entity gridNodeEntityPrefab =  state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<GridNode>(gridNodeEntityPrefab);
            
            // Create grid map.
            GridMap gridMap = new GridMap();
            gridMap.gridEntityArray = new NativeArray<Entity>(totalNodes, Allocator.Persistent);

            // Instantiate grid nodes.
            state.EntityManager.Instantiate(gridNodeEntityPrefab, gridMap.gridEntityArray);
            
            // Set up grid nodes.
            for (int x=0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = CalculateIndex(x, y, width);
                    GridNode gridNode = new GridNode()
                    {
                        x = x,
                        y = y,
                    };
                    state.EntityManager.SetName(gridMap.gridEntityArray[index], $"GridNode {x},{y}");
                    SystemAPI.SetComponent(gridMap.gridEntityArray[index], gridNode);
                }
            }
            
            // Add GridSystemData component to the system entity.
            state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
            state.EntityManager.SetComponentData(state.SystemHandle,
                new GridSystemData()
                {
                    width = width,
                    height = height,
                    gridNodeSize = gridNodeSize,
                    gridMap = gridMap,
                });
        }

#if !VISUALIZE_GRID        
        [BurstCompile]
#endif
        public void OnUpdate(ref SystemState state)
        {
            GridSystemData gridSystemData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);
#if VISUALIZE_GRID
            GridSystemVisualizer.Instance?.InitializeGrid(gridSystemData);
#endif            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (state.EntityManager.HasComponent<GridSystemData>(state.SystemHandle))
            {
                GridSystemData gridSystemData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);
                if (gridSystemData.gridMap.gridEntityArray.IsCreated)
                {
                    gridSystemData.gridMap.gridEntityArray.Dispose();
                }
            }
        
        }
        
        public static int CalculateIndex(int x, int y, int width)
        {
            return x + y * width;
        }


        public static float3 GetWorldPosition(int x, int y, float gridNodeSize)
        {
            return new float3(x * gridNodeSize, 0, y * gridNodeSize);
        }
    }
    
}

