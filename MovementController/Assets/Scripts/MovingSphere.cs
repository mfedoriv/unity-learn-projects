using System;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 5f;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 40f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 1;

    private Vector3 _velocity;
    private Vector3 _desiredVelocity;
    
    private Rigidbody _body;

    private int _groundContactCount;
    private bool OnGround => _groundContactCount > 0;
    private bool _isDesiredJump;
    private int _jumpPhase;
    private Vector3 _contactNormal;

    private float _minGroundDotProduct;

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        OnValidate();
    }

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        _desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        // Using OR assignment to prevent value setting back to false, if we don't call FixedUpdate in next frame.
        // That way it remains true once enabled until we explicitly set it back to false.
        _isDesiredJump |= Input.GetButtonDown("Jump");
    }

   
    private void OnCollisionEnter(Collision other)
    {
        EvaluateCollision(other);
    }

    private void OnCollisionStay(Collision other)
    {
        EvaluateCollision(other);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= _minGroundDotProduct)
            {
                _groundContactCount += 1;
                _contactNormal += normal;
            }
        }
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
    }

    private void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);
        
        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);

        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void FixedUpdate()
    {
        UpdateState();
        
        AdjustVelocity();

        if (_isDesiredJump)
        {
            _isDesiredJump = false;
            Jump();
        }

        _body.velocity = _velocity;
        ClearState();
    }

    private void UpdateState()
    {
        _velocity = _body.velocity;
        if (OnGround)
        {
            _jumpPhase = 0;
            if (_groundContactCount > 1)
            {
                _contactNormal.Normalize();
            }
        }
        else
        {
            _contactNormal = Vector3.up;
        }
    }

    private void ClearState()
    {
        _groundContactCount = 0;
        _contactNormal = Vector3.zero;
    }

    private void Jump()
    {
        if (!OnGround && _jumpPhase >= maxAirJumps) return;
        _jumpPhase += 1;
        // v = sqrt(-2 * g * h) where g - gravity, h - desired height
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        
        float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);
        // to never exceed the jump speed
        if (alignedSpeed > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0);
        }
        
        _velocity += _contactNormal * jumpSpeed;
    }
}