using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Jobs;

public class BoidsBehaviour : MonoBehaviour
{
    [BoxGroup("Common")]
    [SerializeField] private int _countCreature;

    [BoxGroup("Common")]
    [SerializeField] private GameObject _prefab;

    [BoxGroup("Movement")]
    [SerializeField] private float _velocityLimit;

    [BoxGroup("Acceleration")]
    [SerializeField] private float _destinationThreshold;

    [BoxGroup("Acceleration")]
    [SerializeField] private Vector3 _accelerationsWeights;

    [BoxGroup("Bound")]
    [SerializeField] private Vector3 _areaSize;

    private NativeArray<Vector3> _position;
    private NativeArray<Vector3> _speed;
    private NativeArray<Vector3> _accelerations;

    private TransformAccessArray _transformAccessArray;




    private void Start()
    {
        InitializeNativeArrays();
        InitializeTransformAccessArray();
    }

    private void OnDestroy()
    {
        _position.Dispose();
        _speed.Dispose();
        _accelerations.Dispose();

        _transformAccessArray.Dispose();
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
            speeds = _speed,
            accelerations = _accelerations,
            destinationThreshold = _destinationThreshold,
            weights = _accelerationsWeights
        };

        Movement movement = new Movement()
        {
            positions = _position,
            speeds = _speed,
            accelerations = _accelerations,
            deltaTime = Time.deltaTime,
            velocityLimit = _velocityLimit
        };

        JobHandle boundsHandle = bounds.Schedule(_countCreature, 0);
        JobHandle accelerationHandle = acceleration.Schedule(_countCreature , 0, boundsHandle);
        JobHandle movementHandle = movement.Schedule(_transformAccessArray, accelerationHandle);

        movementHandle.Complete();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, _areaSize);
    }

    private void InitializeNativeArrays()
    {
        _position = new NativeArray<Vector3>(_countCreature, Allocator.Persistent);
        _speed = new NativeArray<Vector3>(_countCreature, Allocator.Persistent);
        _accelerations = new NativeArray<Vector3>(_countCreature, Allocator.Persistent);
    }

    private void InitializeTransformAccessArray()
    {
        Transform[] transformsArray = new Transform[_countCreature];

        for (int i = 0; i < _countCreature; i++)
        {
            transformsArray[i] = Instantiate(_prefab).transform;
            _speed[i] = Random.insideUnitSphere;
        }

        _transformAccessArray = new TransformAccessArray(transformsArray);
    }
}
