using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Paddle : MonoBehaviour
{
    [SerializeField, Min(0f)] private float
        minExtents = 4f,
        maxExtents = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f;

    [SerializeField] private bool isAI;

    [SerializeField] private TextMeshPro scoreText;

    [SerializeField] private MeshRenderer goalRenderer;

    [SerializeField, ColorUsage(true, true)]
    private Color goalColor = Color.white;

    [SerializeField, Min(1f)] private float scoreColorIntencity = 3;
    private int _score;
    private float _extents, _targetingBias;

    private static readonly int
        EmissionColorId = Shader.PropertyToID("_EmissionColor"),
        FaceColorId = Shader.PropertyToID("_FaceColor"),
        TimeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");

    private Material _goalMaterial, _paddleMaterial, _scoreMaterial;

    private void Awake()
    {
        _goalMaterial = goalRenderer.material;
        _goalMaterial.SetColor(EmissionColorId, goalColor);
        _paddleMaterial = GetComponent<MeshRenderer>().material;
        _scoreMaterial = scoreText.fontMaterial;
        SetScore(0);
    }

    void SetScore(int newScore, float pointsToWin = 1000f)
    {
        _score = newScore;
        scoreText.SetText("{0}", newScore);
        _scoreMaterial.SetColor(FaceColorId, goalColor * (newScore / pointsToWin) * scoreColorIntencity);
        // Shrink extents of a paddle when scoring
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
    }

    private void ChangeTargetingBias()
    {
        _targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);
    }

    private void SetExtents(float newExtents)
    {
        _extents = newExtents;
        Vector3 s = transform.localScale;
        s.x = 2f * newExtents;
        transform.localScale = s;
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        _goalMaterial.SetFloat(TimeOfLastHitId, Time.time);
        SetScore(_score + 1, pointsToWin);
        return _score >= pointsToWin;
    }

    public void Move(float target, float arenaExtents)
    {
        Vector3 p = transform.localPosition;
        p.x = isAI ? AdjustByAI(p.x, target) : AdjustByPlayer(p.x);
        float limit = arenaExtents - _extents;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        transform.localPosition = p;

    }

    private float AdjustByAI(float x, float target)
    {
        target += _targetingBias * _extents;
        if (x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }

        return Mathf.Max(x - speed * Time.deltaTime, target);
    }

    private float AdjustByPlayer(float x)
    {
        bool goRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool goLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }

        return x;
    }

    public bool HitBall(float ballX, float ballExtents, out float hitFactor)
    {
        ChangeTargetingBias();
        hitFactor = (ballX - transform.localPosition.x) / (_extents + ballExtents);
        bool isSuccess = -1f <= hitFactor && hitFactor <= 1f;
        if (isSuccess)
        {
            _paddleMaterial.SetFloat(TimeOfLastHitId, Time.time);
        }
        return isSuccess;
    }
}
