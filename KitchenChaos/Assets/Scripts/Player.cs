using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 45f;
    [SerializeField] private float deceleration = 20f;

    private bool _isWalking;
    private Vector3 _currentVelocity;
    
    private void Update()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

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
