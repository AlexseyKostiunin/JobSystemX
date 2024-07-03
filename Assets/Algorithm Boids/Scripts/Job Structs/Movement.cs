using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct Movement : IJobParallelForTransform
{
    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _speeds;
    private NativeArray<Vector3> _accelerations;

    private float _deltaTime;
    private float _speedLimit;




    public Movement(
        NativeArray<Vector3> positions, 
        NativeArray<Vector3> speeds, 
        NativeArray<Vector3> accelerations, 
        float deltaTime, 
        float speedLimit)
    {
        _positions = positions;
        _speeds = speeds;
        _accelerations = accelerations;
        _deltaTime = deltaTime;
        _speedLimit = speedLimit;
    }




    private void ResetAcceleration(int index)
    {
        _accelerations[index] = Vector3.zero;
    }

    private Vector3 LimitSpeed(Vector3 acceleration, Vector3 direction)
    {
        return direction * Mathf.Clamp(acceleration.magnitude, 1, _speedLimit);
    }




    public void Execute(int index, TransformAccess transform)
    {
        Vector3 speedCurrentCreature = _speeds[index] + _accelerations[index] * _deltaTime;
        Vector3 direction = speedCurrentCreature.normalized;

        speedCurrentCreature = LimitSpeed(speedCurrentCreature, direction);

        transform.position += speedCurrentCreature * _deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        _positions[index] = transform.position;
        _speeds[index] = speedCurrentCreature;

        ResetAcceleration(index);
    }
}
