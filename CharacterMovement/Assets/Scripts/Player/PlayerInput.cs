using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 inputMove = new Vector2()
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };

            Vector3 movementDirection = Vector3.zero;
            movementDirection += Vector3.right * inputMove.x;
            movementDirection += Vector3.forward * inputMove.y;

            if (_character.camera)
            {
                movementDirection = movementDirection.relativeTo(_character.cameraTransform);
            }
            
            _character.SetMovementDirection(movementDirection);

            // Crouch input
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
            {
                _character.Crouch();
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
            {
                _character.UnCrouch();
            }
            
            // Jump Input
            if (Input.GetButtonDown("Jump"))
            {
                _character.Jump();
            }
            else if (Input.GetButtonUp("Jump"))
            {
                _character.StopJumping();
            }
            
            
        }
    }
}

