using System;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;

    [SerializeField, Range(10, 100)] private int resolution = 10;

    [SerializeField] private FunctionLibrary.FunctionName function;

    private Transform[] _points;

    private void Awake()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        _points = new Transform[resolution * resolution];
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = _points[i] = Instantiate(pointPrefab, transform, false);
            point.localScale = scale;
        }

    }

    private void Update()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            
            _points[i].localPosition = f(u, v, time);

        }
    }
}
