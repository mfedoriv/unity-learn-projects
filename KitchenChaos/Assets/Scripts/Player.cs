using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 45f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private LayerMask countersLayerMask;
    
    private bool _isWalking;
    private Vector3 _currentVelocity;
    private Vector3 _lastInteractDirection;
    
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDirection != Vector3.zero)
        {
            _lastInteractDirection = moveDirection;
        }
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, _lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
             if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) 
             {
                clearCounter.Interact();
             }
        }
        else
        {
            Debug.Log("-");
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = maxSpeed * Time.deltaTime;
        float playerRadius = .65f;
        float playerHeight = 2f;
        var position = transform.position;
        bool canMove = !Physics.CapsuleCast(position, position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance);
        
        // This code is very confusing. Do i really need to do this with a capsule cast or i can do this with usual collider?
        // UPD: Actually, this is some like of Collide and Slide algorithm
        // more about: https://www.youtube.com/watch?v=YR6Q7dUz2uk&t=617s
        if (!canMove)
        {
            // If can't move towards moveDirection (collisions detection)
            // Attempt  only X movement
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(position, position + Vector3.up * playerHeight, playerRadius, moveDirectionX, moveDistance);

            if (canMove)
            { 
                // Can move only on X
                moveDirection = moveDirectionX;
            }
            else
            {
                // Can't move only on X
                // Attempt only Z movement

                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = !Physics.CapsuleCast(position, position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, moveDistance);
                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
                else
                {
                    // Can't move in any direction
                }

            }
        }
        if (canMove)
        {
            // HandleKinematicMovement(moveDirection); // Should be updated in order to work with player's collisions
            transform.position += moveDirection * moveDistance;
        }


        _isWalking = moveDirection != Vector3.zero;
        
        float rotateSpeed = 15f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        
        // Debug.Log(Time.deltaTime);
    }

    private void HandleKinematicMovement(Vector3 moveDirection)
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
