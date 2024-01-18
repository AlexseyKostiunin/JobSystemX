using Unity.Collections;
using Unity.Burst;
using UnityEngine;
using Unity.Jobs;
using System;

[BurstCompile]
public struct Bounds : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<Vector3> positions;

    public NativeArray<Vector3> accelerations;

    public Vector3 areaSize;




    private Vector3 Compensate(float delta, Vector3 direction)
    {
        float threshold = 3f;
        float multiplier = 100f;
        delta = Math.Abs(delta);

        if (delta > threshold)
            return Vector3.zero;

        return direction * (1 - delta / threshold) * multiplier;
    }




    public void Execute(int index)
    {
        Vector3 position = positions[index];
        Vector3 size = areaSize * 0.5f;

        accelerations[index] +=
            Compensate(-size.x - position.x, Vector3.right) +
            Compensate(size.x - position.x, Vector3.left) +
            Compensate(-size.y - position.y, Vector3.up) +
            Compensate(size.y - position.y, Vector3.down) +
            Compensate(-size.z - position.z, Vector3.forward) +
            Compensate(size.z - position.z, Vector3.back);
    }
}
