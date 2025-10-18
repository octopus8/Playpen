using UnityEngine;
using Unity.Entities;

public static class InfiniteLoopProtection
{
    private static int iterationCount = 0;

    public static void CheckIterationCount(WorldUnmanaged world, int maxIterations)
    {
        iterationCount++;
        if (iterationCount > maxIterations)
        {
            Debug.LogError($"Infinite loop detected: exceeded {maxIterations} iterations.");
            throw new System.Exception("Infinite loop protection triggered.");
        }
    }

    public static void Reset()
    {
        iterationCount = 0;
    }
}