using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)] private float
        maxXSpeed = 20f,
        maxStartXSpeed = 2f,
        constantYSpeed = 10f,
        extents = 0.5f;

    private Vector2 _position, _velocity;

    public Vector2 Velocity => _velocity;

    public float Extents => extents;

    public Vector2 Position => _position;

    public void UpdateVisualisation() => transform.localPosition = new Vector3(_position.x, 0f, _position.y);

    public void Move() => _position += _velocity * Time.deltaTime;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        _position = Vector2.zero;
        UpdateVisualisation();
        _velocity.x = Random.Range(-maxXSpeed, maxStartXSpeed);
        _velocity.y = -constantYSpeed;
        gameObject.SetActive(true);
    }

    public void EndGame()
    {
        _position.x = 0f;
        gameObject.SetActive(false);
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        _velocity.x = maxXSpeed * speedFactor;
        _position.x = start + _velocity.x * deltaTime;
    }

    public void BounceX(float boundary)
    {
        _position.x = 2f * boundary - _position.x;
        _velocity.x = -_velocity.x;
    }

    public void BounceY(float boundary)
    {
        _position.y = 2f * boundary - _position.y;
        _velocity.y = -_velocity.y;
    }

}