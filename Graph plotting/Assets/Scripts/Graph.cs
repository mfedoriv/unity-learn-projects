using System;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;

    [SerializeField, Range(10, 100)] private int resolution = 10;

    private Transform[] _points;

    private void Awake()
    {
        float step = 2f / resolution;
        var position = Vector3.zero;
        var scale = Vector3.one * step;
        _points = new Transform[resolution];
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = _points[i] = Instantiate(pointPrefab, transform, false);
            position.x = (i + 0.5f) * step - 1f; // filling (-1,1) range
            point.localPosition = position;
            point.localScale = scale;
        }

    }

    private void Update()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = _points[i];
            Vector3 position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time));
            point.localPosition = position;

        }
    }
}
