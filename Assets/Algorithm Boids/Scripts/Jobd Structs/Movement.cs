using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct Movement : IJobParallelForTransform
{
    public NativeArray<Vector3> positions;
    public NativeArray<Vector3> velocities;
    public NativeArray<Vector3> accelerations;

    public float deltaTime;
    public float velocityLimit;




    public void Execute(int index, TransformAccess transform)
    {
        Vector3 velocity = velocities[index] + accelerations[index] * deltaTime;
        Vector3 direction = velocity.normalized;

        velocity = direction * Mathf.Clamp(velocity.magnitude, 1, velocityLimit);
        transform.position += velocity * deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        positions[index] = transform.position;
        velocities[index] = velocity;
        accelerations[index] = Vector3.zero;
    }
}
