using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 45f;
    [SerializeField] private float deceleration = 20f;

    private bool _isWalking;
    private Vector3 _currentVelocity;
    
    private void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1;
        }
        inputVector = inputVector.normalized;

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        HandleMovement(moveDirection);

        _isWalking = moveDirection != Vector3.zero;
        
        float rotateSpeed = 15f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        
        // Debug.Log(Time.deltaTime);
    }

    private void HandleMovement(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            _currentVelocity += moveDirection * (acceleration * Time.deltaTime);
            _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, maxSpeed);
        }
        else
        {
            _currentVelocity -= _currentVelocity * (deceleration * Time.deltaTime);
        }
        
        transform.position += _currentVelocity * Time.deltaTime;
    }

    public bool IsWalking()
    {
        return _isWalking;
    }
}
