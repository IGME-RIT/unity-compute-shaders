using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectInstancedRendering : MonoBehaviour
{
    public int instanceCount = 1000;

    public Mesh instanceMesh;

    // this time, we will have to use a custom shader to deal with instance rending on the GPU
    public Material instanceMaterial;
    public int subMeshIndex = 0;

    // since transforms will be modified within the GPU, we now need a buffer to pass data to the GPU
    ComputeBuffer transformBuffer;
    Matrix4x4[] instanceTransforms;

    ComputeBuffer argsBuffer;
    uint[] args;

    void Start()
    {
        transformBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 16);
        instanceTransforms = new Matrix4x4[instanceCount];
        GenerateTransforms();

        args = new uint[5] { 0, 0, 0, 0, 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        GenerateArguments();
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    /// <summary>
    /// This method is used to generate random transforms for each of the instances
    /// </summary>
    private void GenerateTransforms()
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        Vector3 scale = new Vector3();
        for (int i = 0; i < instanceCount; i++)
        {
            pos = Random.insideUnitSphere * 50;
            rot = Quaternion.Euler(Random.insideUnitSphere * 180);
            scale = Random.insideUnitSphere;
            instanceTransforms[i] = Matrix4x4.TRS(pos, rot, scale);
        }
        transformBuffer.SetData(instanceTransforms);
        instanceMaterial.SetBuffer("transformBuffer", transformBuffer);
    }

    private void GenerateArguments()
    {
        if (instanceMesh != null)
        {
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1); // make sure submesh index is valid
            args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)instanceCount;
            args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }
        argsBuffer.SetData(args);
    }

    /// <summary>
    /// This is being added because buffers are not automatically released by the garbage collector
    /// </summary>
    private void OnDestroy()
    {
        if(transformBuffer != null)
        {
            transformBuffer.Release();
            transformBuffer = null;
        }

        if(argsBuffer != null)
        {
            argsBuffer.Release();
            argsBuffer = null;
        }
    }
}
