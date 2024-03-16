using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivelyCamera : MonoBehaviour
{
    [SerializeField, Min(0f)] private float
        springStrength = 100f,
        dampingStrength = 10f,
        jostleStrength = 20f,
        pushStrength = 0.5f,
        maxDeltaTime = 1f / 60f;

    private Vector3 _anchorPosition, _velocity;

    private void Awake()
    {
        _anchorPosition = transform.localPosition;
    }

    public void JostleY()
    {
        _velocity.y += jostleStrength;
    }

    public void PushXZ(Vector2 impulse)
    {
        _velocity.x += pushStrength * impulse.x;
        _velocity.z += pushStrength * impulse.y;
    }

    public void LateUpdate()
    {
        float dt = Time.deltaTime;
        while (dt > maxDeltaTime)
        {
            TimeStep(maxDeltaTime);
            dt -= maxDeltaTime;
        }
        TimeStep(dt);
    }

    private void TimeStep(float dt)
    {
        Vector3 displacement = _anchorPosition - transform.localPosition;
        Vector3 acceleration = springStrength * displacement - dampingStrength * _velocity;
        _velocity += acceleration * Time.deltaTime;
        transform.localPosition += _velocity * Time.deltaTime;
    }
}