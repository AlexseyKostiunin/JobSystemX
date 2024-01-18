using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Jobs;

public class BoidsBehaviour : MonoBehaviour
{
    [SerializeField] private int _countCreature;
    [SerializeField] private float _destinationThreshold;

    [SerializeField] private GameObject _prefab;

    [SerializeField] private Vector3 _areaSize;
    [SerializeField] private float _velocityLimit;
    [SerializeField] private Vector3 _accelerationsWeights;

    private NativeArray<Vector3> _position;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _accelerations;

    private TransformAccessArray _transformAccessArray;




    private void Start()
    {
        _position = new NativeArray<Vector3>(_countCreature, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(_countCreature, Allocator.Persistent);
        _accelerations = new NativeArray<Vector3>(_countCreature, Allocator.Persistent);

        Transform[] transformsArray = new Transform[_countCreature];
        for (int i = 0; i < _countCreature; i++)
        {
            transformsArray[i] = Instantiate(_prefab).transform;
            _velocities[i] = Random.insideUnitSphere;
        }

        _transformAccessArray = new TransformAccessArray(transformsArray);
    }

    private void Update()
    {
        Bounds bounds = new Bounds()
        {
            positions = _position,
            accelerations = _accelerations,
            areaSize = _areaSize
        };

        Acceleration acceleration = new Acceleration()
        {
            positions = _position,
            velocities = _velocities,
            accelerations = _accelerations,
            destinationThreshold = _destinationThreshold,
            weights = _accelerationsWeights
        };

        Movement movement = new Movement()
        {
            positions = _position,
            velocities = _velocities,
            accelerations = _accelerations,
            deltaTime = Time.deltaTime,
            velocityLimit = _velocityLimit
        };

        JobHandle boundsHandle = bounds.Schedule(_countCreature, 0);
        JobHandle accelerationHandle = acceleration.Schedule(_countCreature , 0, boundsHandle);
        JobHandle movementHandle = movement.Schedule(_transformAccessArray, accelerationHandle);

        movementHandle.Complete();
    }

    private void OnDestroy()
    {
        _position.Dispose();
        _velocities.Dispose();
        _transformAccessArray.Dispose();
        _accelerations.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, _areaSize);
    }
}
