// Define VISUALIZE_GRID to enable grid visualization.
#define VISUALIZE_GRID

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

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
        public const int WALL_COST = byte.MaxValue;
        public const int FLOW_FIELD_MAP_COUNT = 50;
        
        
        public struct GridSystemData : IComponentData
        {
            public int width;
            public int height;
            public float gridNodeSize;
            public NativeArray<GridMap> gridMapArray;
            public int nextGridIndex;
        }

        public struct GridMap
        {
            public NativeArray<Entity> gridEntityArray;
            public int2 targetGridPosition;
            public bool isValid;
        }
        
        public struct GridNode : IComponentData
        {
            public int index;
            public int x;
            public int y;
            public byte cost;
            public byte bestCost;
            public float2 vector;
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
            
            NativeArray<GridMap> gridMapArray = new NativeArray<GridMap>(FLOW_FIELD_MAP_COUNT, Allocator.Persistent);

            for (int i = 0; i < FLOW_FIELD_MAP_COUNT; i++)
            {
                // Create grid map.
                GridMap gridMap = new GridMap();
                gridMap.isValid = false;
                gridMap.gridEntityArray = new NativeArray<Entity>(totalNodes, Allocator.Persistent);

                // Instantiate grid nodes.
                state.EntityManager.Instantiate(gridNodeEntityPrefab, gridMap.gridEntityArray);

                // Set up grid nodes.
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int index = CalculateIndex(x, y, width);
                        GridNode gridNode = new GridNode()
                        {
                            index = index,
                            x = x,
                            y = y,
                        };
                        state.EntityManager.SetName(gridMap.gridEntityArray[index], $"GridNode {x},{y}");
                        SystemAPI.SetComponent(gridMap.gridEntityArray[index], gridNode);
                    }
                }
                
                gridMapArray[i] = gridMap;
            }

            // Add GridSystemData component to the system entity.
            state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
            state.EntityManager.SetComponentData(state.SystemHandle,
                new GridSystemData()
                {
                    width = width,
                    height = height,
                    gridNodeSize = gridNodeSize,
                    gridMapArray = gridMapArray,
                    nextGridIndex = 0,
                });
        }

#if !VISUALIZE_GRID        
        [BurstCompile]
#endif
        public void OnUpdate(ref SystemState state)
        {
            GridSystemData gridSystemData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);

            foreach (var (
                         flowFieldPathRequest,
                         flowFieldPathRequestEnabled,
                         flowFieldFollower,
                         flowFieldFollowerEnabled) in
                     SystemAPI.Query<
                         RefRO<FlowFieldPathRequest>,
                         EnabledRefRW<FlowFieldPathRequest>,
                         RefRW<FlowFieldFollower>,
                         EnabledRefRW<FlowFieldFollower>
                     >().WithPresent<FlowFieldFollower>())
            {
                int2 targetGridPosition = GetGridPosition(flowFieldPathRequest.ValueRO.targetPosition,
                    gridSystemData.gridNodeSize);

                flowFieldPathRequestEnabled.ValueRW = false;

                bool alreadyCalculatedPath = false;
                for (int i = 0; i < FLOW_FIELD_MAP_COUNT; ++i)
                {
                    if (gridSystemData.gridMapArray[i].isValid &&
                        gridSystemData.gridMapArray[i].targetGridPosition.Equals(targetGridPosition))
                    {
                        flowFieldFollower.ValueRW.gridIndex = i;
                        flowFieldFollower.ValueRW.targetPosition = flowFieldPathRequest.ValueRO.targetPosition;
                        flowFieldFollowerEnabled.ValueRW = true;
                        
                        alreadyCalculatedPath = true;
                        break;
                    }
                }
                if (alreadyCalculatedPath)
                {
                    continue;
                }
                

                int gridIndex = gridSystemData.nextGridIndex;
                gridSystemData.nextGridIndex = (gridSystemData.nextGridIndex + 1) % FLOW_FIELD_MAP_COUNT;
                SystemAPI.SetComponent(state.SystemHandle, gridSystemData);
                
                Debug.Log("Calculating path to " + targetGridPosition + " :: " + gridIndex);
                flowFieldFollower.ValueRW.gridIndex = gridIndex;
                flowFieldFollower.ValueRW.targetPosition = flowFieldPathRequest.ValueRO.targetPosition;
                flowFieldFollowerEnabled.ValueRW = true;
                

                NativeArray<RefRW<GridNode>> gridNodeArray =
                    new NativeArray<RefRW<GridNode>>(gridSystemData.width * gridSystemData.height, Allocator.Temp);

                // Initialize grid nodes with cost and bestCost values.
                for (int x = 0; x < gridSystemData.width; x++)
                {
                    for (int y = 0; y < gridSystemData.height; y++)
                    {
                        int index = CalculateIndex(x, y, gridSystemData.width);
                        Entity entity = gridSystemData.gridMapArray[gridIndex].gridEntityArray[index];
                        RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(entity);
                        gridNodeArray[index] = gridNode;

                        // If this is the target grid position, set cost and bestCost to 0, otherwise set cost to 1 and bestCost to max value.
                        gridNode.ValueRW.vector = new float2(0, 1);
                        if (x == targetGridPosition.x && y == targetGridPosition.y)
                        {
                            gridNode.ValueRW.cost = 0;
                            gridNode.ValueRW.bestCost = 0;
                        }
                        else
                        {
                            gridNode.ValueRW.cost = 1;
                            gridNode.ValueRW.bestCost = byte.MaxValue;
                        }
                    }
                }


                PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
                NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

                for (int x = 0; x < gridSystemData.width; x++)
                {
                    for (int y = 0; y < gridSystemData.height; y++)
                    {
                        if (collisionWorld.OverlapSphere(
                                GetWorldCenterPosition(x, y, gridSystemData.gridNodeSize),
                                gridSystemData.gridNodeSize * 0.5f,
                                ref hits,
                                new CollisionFilter()
                                {
                                    BelongsTo = ~0u,
                                    CollidesWith = (1u << RTSGame.PATHFINDING_WALL_LAYER),
                                    GroupIndex = 0,
                                }
                            ))
                        {
                            int index = CalculateIndex(x, y, gridSystemData.width);
                            gridNodeArray[index].ValueRW.cost = WALL_COST;
                        }

                        hits.Clear();
                    }
                }
                hits.Dispose();
                
                NativeQueue<RefRW<GridNode>> processingQueue = new NativeQueue<RefRW<GridNode>>(Allocator.Temp);
                RefRW<GridNode> targetGridNode =
                    gridNodeArray[CalculateIndex(targetGridPosition, gridSystemData.width)];
                processingQueue.Enqueue(targetGridNode);

                while (processingQueue.Count > 0)
                {
                    // Infinite loop protection.
                    InfiniteLoopProtection.CheckIterationCount(state.WorldUnmanaged, 10000);

                    // Dequeue the next node to process.
                    RefRW<GridNode> currentNode = processingQueue.Dequeue();

                    // Get the current node's grid position.
                    int2 currentPos = new int2(currentNode.ValueRW.x, currentNode.ValueRW.y);

                    // Check neighbors (up, down, left, right)
                    int2[] neighborOffsets = new int2[]
                    {
                        new int2(0, 1),
                        new int2(0, -1),
                        new int2(1, 0),
                        new int2(-1, 0),
                        new int2(1, 1),
                        new int2(1, -1),
                        new int2(-1, 1),
                        new int2(-1, -1),
                    };
                    foreach (int2 offset in neighborOffsets)
                    {

                        // Get neighbor position.
                        int2 neighborPos = currentPos + offset;

                        // If neighbor position is valid, process it.
                        if (IsValidGridPosition(neighborPos, gridSystemData.width, gridSystemData.height))
                        {
                            // Get neighbor node.
                            int neighborIndex = CalculateIndex(neighborPos, gridSystemData.width);
                            RefRW<GridNode> neighborNode = gridNodeArray[neighborIndex];

                            if (neighborNode.ValueRW.cost == WALL_COST)
                            {
                                continue;
                            }

                            // Calculate new best cost for neighbor.
                            byte newBestCost = (byte)(currentNode.ValueRW.bestCost + neighborNode.ValueRW.cost);

                            // If new best cost is lower, update neighbor node and enqueue it for processing.
                            if (newBestCost < neighborNode.ValueRW.bestCost)
                            {
                                neighborNode.ValueRW.bestCost = newBestCost;
                                neighborNode.ValueRW.vector = new float2(currentPos.x - neighborPos.x,
                                    currentPos.y - neighborPos.y);
                                processingQueue.Enqueue(neighborNode);
                            }
                        }
                    }
                }

                // Reset infinite loop protection counter.
                InfiniteLoopProtection.Reset();

                // Dispose of temporary arrays.
                gridNodeArray.Dispose();
                processingQueue.Dispose();

                GridMap gridMap = gridSystemData.gridMapArray[gridIndex];
                gridMap.targetGridPosition = targetGridPosition;
                gridMap.isValid = true;
                gridSystemData.gridMapArray[gridIndex] = gridMap;
                SystemAPI.SetComponent(state.SystemHandle, gridSystemData);
            }

            if (Input.GetMouseButtonDown(0))
            {
                float3 mouseWorldPosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
                int2 gridPosition = GetGridPosition(mouseWorldPosition, gridSystemData.gridNodeSize);
                if (IsValidGridPosition(gridPosition, gridSystemData.width,  gridSystemData.height))
                {
/*                    
                    int index = CalculateIndex(gridPosition.x, gridPosition.y, gridSystemData.width);
                    Entity entity = gridSystemData.gridMapArray.gridEntityArray[index];
                    RefRW<GridNode> gridNode = SystemAPI.GetComponentRW<GridNode>(entity);
*/                    
                }
            }
            
#if VISUALIZE_GRID
            GridSystemVisualizer.Instance?.InitializeGrid(gridSystemData);
            GridSystemVisualizer.Instance?.UpdateGrid(gridSystemData);
#endif            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (state.EntityManager.HasComponent<GridSystemData>(state.SystemHandle))
            {
                RefRW<GridSystemData> gridSystemData = SystemAPI.GetComponentRW<GridSystemData>(state.SystemHandle);
                for (int i = 0; i < FLOW_FIELD_MAP_COUNT; ++i)
                {
                    if (gridSystemData.ValueRW.gridMapArray[i].gridEntityArray.IsCreated)
                    {
                        gridSystemData.ValueRW.gridMapArray[i].gridEntityArray.Dispose();
                    }
                }

                gridSystemData.ValueRW.gridMapArray.Dispose();
            }
        }
        
        
        public static int CalculateIndex(int x, int y, int width)
        {
            return x + y * width;
        }
        
        public static int CalculateIndex(int2 position, int width)
        {
            return CalculateIndex(position.x, position.y, width);
        }


        public static float3 GetWorldPosition(int x, int y, float gridNodeSize)
        {
            return new float3(x * gridNodeSize, 0, y * gridNodeSize);
        }
        
        public static float3 GetWorldCenterPosition(int x, int y, float gridNodeSize)
        {
            return new float3(
                x * gridNodeSize + gridNodeSize * 0.5f,
                0,
                y * gridNodeSize + gridNodeSize * 0.5f);
        }
        
        public static int2 GetGridPosition(float3 worldPosition, float gridNodeSize)
        {
            return new int2((int)math.floor(worldPosition.x / gridNodeSize), (int)math.floor(worldPosition.z / gridNodeSize));
        }

        public static bool IsValidGridPosition(int2 gridPosition, int width, int height)
        {
            return gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < width && gridPosition.y < height;
        }
        
        public static float3 GetWorldMovementVector(float2 vector)
        {
            return new float3(vector.x, 0, vector.y);
        }

        public static bool IsWall(GridNode gridNode)
        {
            if (gridNode.cost == WALL_COST)
            {
                return true;
            }
            return false;
        }
        
    }
    
}

