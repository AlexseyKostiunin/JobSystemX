using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct Movement : IJobParallelForTransform
{
    public NativeArray<Vector3> positions;
    public NativeArray<Vector3> speeds;
    public NativeArray<Vector3> accelerations;

    public float deltaTime;
    public float speedLimit;




    private void ResetAcceleration(int index)
    {
        accelerations[index] = Vector3.zero;
    }

    private Vector3 LimitSpeed(Vector3 acceleration, Vector3 direction)
    {
        return direction * Mathf.Clamp(acceleration.magnitude, 1, speedLimit);
    }




    public void Execute(int index, TransformAccess transform)
    {
        Vector3 speedCurrentCreature = speeds[index] + accelerations[index] * deltaTime;
        Vector3 direction = speedCurrentCreature.normalized;

        speedCurrentCreature = LimitSpeed(speedCurrentCreature, direction);

        transform.position += speedCurrentCreature * deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        positions[index] = transform.position;
        speeds[index] = speedCurrentCreature;

        ResetAcceleration(index);
    }
}
