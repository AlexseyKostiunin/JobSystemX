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




    public Acceleration(
        NativeArray<Vector3> positions, 
        NativeArray<Vector3> speeds, 
        NativeArray<Vector3> accelerations, 
        Vector3 weights, 
        float destinationThreshold)
    {
        this.positions = positions;
        this.speeds = speeds;
        this.accelerations = accelerations;
        this.weights = weights;
        this.destinationThreshold = destinationThreshold;
    }




    private int _totalNumbersCreatures => positions.Length - 1;




    public void Execute(int index)
    {
        Vector3 averageSpread = Vector3.zero;
        Vector3 averageSpeed = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;

        for (int i = 0; i < _totalNumbersCreatures; i++)
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
            (averageSpread / _totalNumbersCreatures) * weights.x +
            (averageSpeed / _totalNumbersCreatures) * weights.y +
            (averagePosition / _totalNumbersCreatures - positions[index]) * weights.z;
    }
}
