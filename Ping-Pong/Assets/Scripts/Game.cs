using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Ball ball;

    [SerializeField] private Paddle bottomPaddle, topPaddle;

    [SerializeField, Min(0f)] private Vector2 arenaExtents = new Vector2(10f, 10f);

    [SerializeField, Min(2)] private int pointsToWin = 3;

    [SerializeField] private TextMeshPro countdownText;

    [SerializeField, Min(1f)] private float newGameDelay = 3f;

    [SerializeField] private LivelyCamera livelyCamera;

    private float _countdownUntilNewGame;

    private void Awake()
    {
        Application.targetFrameRate = 0;
        _countdownUntilNewGame = newGameDelay;
    }

    void StartNewGame()
    {
        ball.StartNewGame();
        bottomPaddle.StartNewGame();
        topPaddle.StartNewGame();
    }

    void Update()
    {
        bottomPaddle.Move(ball.Position.x, arenaExtents.x);
        topPaddle.Move(ball.Position.x, arenaExtents.x);
        if (_countdownUntilNewGame <= 0f)
        {
            UpdateGame();
        }
        else
        {
            UpdateCountdown();
        }
    }

    private void UpdateGame()
    {
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualisation();
    }

    private void UpdateCountdown()
    {
        _countdownUntilNewGame -= Time.deltaTime;
        if (_countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(_countdownUntilNewGame);
            if (displayValue < newGameDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }

    private void EndGame()
    {
        _countdownUntilNewGame = newGameDelay;
        countdownText.SetText("GAME OVER");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
    }

    private void BounceYIfNeeded()
    {
        float yExtents = arenaExtents.y - ball.Extents;
        if (ball.Position.y < -yExtents)
        {
            BounceY(-yExtents, bottomPaddle, topPaddle);
        }
        else if (ball.Position.y > yExtents)
        {
            BounceY(yExtents, topPaddle, bottomPaddle);
        }
    }

    private void BounceY(float boundary, Paddle defender, Paddle attacker)
    {
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        float bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;

        BounceXIfNeeded(bounceX);
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        livelyCamera.PushXZ(ball.Velocity);
        
        ball.BounceY(boundary);

        if (defender.HitBall(bounceX, ball.Extents, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else
        {
            livelyCamera.JostleY();
            if (attacker.ScorePoint(pointsToWin))
            {
                EndGame();
            }
        }
    }

    private void BounceXIfNeeded(float x)
    {
        float xExtents = arenaExtents.x - ball.Extents;
        if (x < -xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(-xExtents);
        }
        else if (x > xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(xExtents);
        }
    }
}