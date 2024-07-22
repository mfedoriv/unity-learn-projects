using System;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 20f, maxAirAcceleration = 10f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 1;

    private Vector3 _velocity;
    private Vector3 _desiredVelocity;
    
    private Rigidbody _body;

    private bool _onGround;
    private bool _isDesiredJump;
    private int _jumpPhase;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
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
            _onGround |= normal.y > 0.9f;
        }
    }

    private void FixedUpdate()
    {
        UpdateState();
        float acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, maxSpeedChange);

        if (_isDesiredJump)
        {
            _isDesiredJump = false;
            Jump();
        }

        _body.velocity = _velocity;
        _onGround = false;
    }

    private void UpdateState()
    {
        _velocity = _body.velocity;
        if (_onGround)
        {
            _jumpPhase = 0;
        }
    }

    private void Jump()
    {
        if (!_onGround && _jumpPhase >= maxAirJumps) return;
        _jumpPhase += 1;
        // v = sqrt(-2 * g * h) where g - gravity, h - desired height
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        // to never exceed the jump speed
        if (_velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0);
        }
        
        _velocity.y += jumpSpeed;
    }
}