using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 20f;
    [SerializeField] private Rect allowedArea = new Rect(-4.5f, -4.5f, 9f, 9f);

    [SerializeField, Range(0f, 1f)] private float bounciness = 0.5f;


    private Vector3 _velocity;

    // Update is called once per frame
    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);
        Vector3 displacement = _velocity * Time.deltaTime;
        
        Vector3 newPosition = transform.localPosition + displacement;
        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            _velocity.x = -_velocity.x * bounciness;
        } else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            _velocity.x = -_velocity.x * bounciness;
        } else if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            _velocity.z = -_velocity.z * bounciness;
        } else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            _velocity.z = -_velocity.z * bounciness;
        }
        transform.localPosition = newPosition;
    }
}