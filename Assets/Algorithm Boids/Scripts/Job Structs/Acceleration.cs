using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct Acceleration : IJobParallelFor
{
    [ReadOnly]
    private NativeArray<Vector3> _positions;

    [ReadOnly]
    private NativeArray<Vector3> _speeds;
    private NativeArray<Vector3> _accelerations;

    private Vector3 _weights;

    private float _destinationThreshold;




    public Acceleration(
        NativeArray<Vector3> positions, 
        NativeArray<Vector3> speeds, 
        NativeArray<Vector3> accelerations, 
        Vector3 weights, 
        float destinationThreshold)
    {
        _positions = positions;
        _speeds = speeds;
        _accelerations = accelerations;
        _weights = weights;
        _destinationThreshold = destinationThreshold;
    }




    private int TotalNumbersCreatures => _positions.Length - 1;




    public void Execute(int index)
    {
        Vector3 averageSpread = Vector3.zero;
        Vector3 averageSpeed = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;

        for (int i = 0; i < TotalNumbersCreatures; i++)
        {
            if (i == index)
                continue;

            Vector3 targetPosition = _positions[i];
            Vector3 positionDifference = _positions[index] - targetPosition;

            if (positionDifference.magnitude > _destinationThreshold)
                continue;

            averageSpread += positionDifference.normalized;
            averageSpeed += _speeds[i];
            averagePosition += targetPosition;
        }

        _accelerations[index] += 
            (averageSpread / TotalNumbersCreatures) * _weights.x +
            (averageSpeed / TotalNumbersCreatures) * _weights.y +
            (averagePosition / TotalNumbersCreatures - _positions[index]) * _weights.z;
    }
}
