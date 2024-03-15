using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)] private float
        maxXSpeed = 20f;

    [FormerlySerializedAs("constantXSpeed")] [SerializeField, Min(0f)] private float
        startXSpeed = 8f;

    [SerializeField, Min(0f)] private float
        constantYSpeed = 10f,
        extents = 0.5f;

    private Vector2 position, velocity;

    public Vector2 Velocity => velocity;

    public float Extents => extents;

    public Vector2 Position => position;

    public void UpdateVisualisation() => transform.localPosition = new Vector3(position.x, 0f, position.y);

    public void Move() => position += velocity * Time.deltaTime;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualisation();
        velocity = new Vector2(startXSpeed, constantYSpeed);
        gameObject.SetActive(true);
    }

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }

    public void BounceX(float boundary)
    {
        position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
    }

    public void BounceY(float boundary)
    {
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
    }

}