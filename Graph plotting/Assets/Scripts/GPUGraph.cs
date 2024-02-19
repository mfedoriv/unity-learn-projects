using System;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;
    
    [SerializeField, Range(10, 1000)] private int resolution = 200;

    [SerializeField] private FunctionLibrary.FunctionName function;
    [SerializeField, Min(0f)] private float functionDuration = 1f, transitionDuration = 1f;
    
    public enum TransitionMode { Cycle, Random }
    [SerializeField] private TransitionMode transitionMode;

    private float _duration;

    private bool transitioning;

    private FunctionLibrary.FunctionName transitionFunction;

    private ComputeBuffer positionsBuffer;
    private static readonly int PositionsId = Shader.PropertyToID("_Positions"),
        ResolutionId = Shader.PropertyToID("_Resolution"),
        StepId = Shader.PropertyToID("_Step"),
        TimeId = Shader.PropertyToID("_Time");

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(ResolutionId, resolution);
        computeShader.SetFloat(StepId, step);
        computeShader.SetFloat(TimeId, Time.time);
        
        computeShader.SetBuffer(0, PositionsId, positionsBuffer);
        
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);
        
        material.SetBuffer(PositionsId, positionsBuffer);
        material.SetFloat(StepId, step);
        
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4); // stride - size of element in bytes
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }


    private void Update()
    {
        _duration += Time.deltaTime;
        if (transitioning)
        {
            if (_duration >= transitionDuration)
            {
                _duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (_duration >= functionDuration)
        {
            _duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        UpdateFunctionOnGPU();
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle
            ? FunctionLibrary.GetNextFunctionName(function)
            : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}
