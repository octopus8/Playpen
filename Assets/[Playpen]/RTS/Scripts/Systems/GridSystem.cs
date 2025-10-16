using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS
{

    partial struct GridSystem : ISystem
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
        
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            int width = 20;
            int height = 10;
            float gridNodeSize = 5f;
            int totalNodes = width * height;

            Entity gridNodeEntityPrefab =  state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<GridNode>(gridNodeEntityPrefab);
            
            
            GridMap gridMap = new GridMap();
            gridMap.gridEntityArray = new NativeArray<Entity>(totalNodes, Allocator.Persistent);
            
            state.EntityManager.Instantiate(gridNodeEntityPrefab, gridMap.gridEntityArray);
            
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        
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
    }
    
}

