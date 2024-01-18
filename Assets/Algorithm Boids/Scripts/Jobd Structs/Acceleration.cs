using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct Acceleration : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<Vector3> positions;

    [ReadOnly]
    public NativeArray<Vector3> speeds;

    public NativeArray<Vector3> accelerations;

    public Vector3 weights;

    public float destinationThreshold;




    private int TotalNumbersCreatures => positions.Length - 1;




    public void Execute(int index)
    {
        Vector3 averageSpread = Vector3.zero;
        Vector3 averageSpeed = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;

        for (int i = 0; i < TotalNumbersCreatures; i++)
        {
            if (i == index)
                continue;

            Vector3 targetPosition = positions[i];
            Vector3 positionDifference = positions[index] - targetPosition;

            if (positionDifference.magnitude > destinationThreshold)
                continue;

            averageSpread += positionDifference.normalized;
            averageSpeed += speeds[i];
            averagePosition += targetPosition;
        }

        accelerations[index] += 
            (averageSpread / TotalNumbersCreatures) * weights.x +
            (averageSpeed / TotalNumbersCreatures) * weights.y +
            (averagePosition / TotalNumbersCreatures - positions[index]) * weights.z;
    }
}
