using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class CubeGeometryShaderController: MonoBehaviour
{
    public int sqrtCubeCount = 5000;
    public float cubeSize = 1.0f;
    public Material material;

    void Start()
    {
        // Initialize the structured buffer
        var numberOfObjectsPerSide = sqrtCubeCount;

        _totalObjects = numberOfObjectsPerSide * numberOfObjectsPerSide;
        
        _centerBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GraphicsBuffer.UsageFlags.LockBufferForWrite, _totalObjects, sizeof(float) * 3);
        var array = _centerBuffer.LockBufferForWrite<float3>(0, _totalObjects);

        for (var i = 0; i < numberOfObjectsPerSide; ++i)
        for (var j = 0; j < numberOfObjectsPerSide; ++j)
            array[i + (j * numberOfObjectsPerSide)] = new float3(i * 1.5f, 0, j * 1.5f);
        // Set the structured buffer and cube size properties in the material
        material.SetBuffer("centers", _centerBuffer);
        material.SetFloat("_CubeSize", cubeSize);

        // Create the cube mesh
        _cubeMesh = new Mesh();
        _cubeMesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f)
        };
        _cubeMesh.triangles = new int[]
        {
            0, 2, 1,
            1, 2, 3,
            1, 3, 5,
            3, 7, 5,
            2, 6, 3,
            3, 6, 7,
            4, 5, 7,
            4, 7, 6,
            0, 4, 2,
            2, 4, 6,
            0, 1, 4,
            1, 5, 4
        };
    }

    void Update()
    {
        _jobhandle.Complete();
        //now that we are sure that the job is complete, we can do end write and start a new write.
        //not sure if this is actually necessary
        _centerBuffer.UnlockBufferAfterWrite<float3>(_totalObjects);
        _array = _centerBuffer.LockBufferForWrite<float3>(0, _totalObjects);

        _jobhandle = new UpdateDataJob(Time.unscaledTime, sqrtCubeCount)
        {
            array = _array
        }.ScheduleBatch(_totalObjects, 32);

        // Draw the cubes
        Graphics.DrawMeshInstancedProcedural(
            _cubeMesh, 0, material, new Bounds(transform.position, Vector3.one * 1000000), sqrtCubeCount * sqrtCubeCount, null);
    }

    void OnDestroy()
    {
        // Release the structured buffer and cube mesh
        _centerBuffer.Release();
        Destroy(_cubeMesh);
    }

    [BurstCompile]
    struct UpdateDataJob: IJobParallelForBatch
    {
        [NoAlias] [NativeDisableParallelForRestriction] public NativeArray<float3> array;

        readonly int sqrtCubeCount;

        public UpdateDataJob(float time, int cubeCount): this()
        {
            _time = time;
            sqrtCubeCount = cubeCount;
        }

        public void Execute(int startIndex, int count)
        {
            for (var index = startIndex + count - 1; index >= startIndex; index--)
            {
                float j = index / sqrtCubeCount;
                float i = index % sqrtCubeCount;
                var y = math.sin(2 * math.PI * (i - _time) / 10);
                array[index] = new float3(i * 1.5f, y, j * 1.5f);
            }
        }

        readonly float _time;
    }

    NativeArray<float3> _array;
    JobHandle _jobhandle;
    int _totalObjects;
    GraphicsBuffer _centerBuffer;
    Mesh _cubeMesh;
}