using Cinemachine;
using ECM2;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow.")]
        [SerializeField]
        private GameObject cameraTarget;

        [Tooltip("How far in degrees can you move the camera up.")]
        [SerializeField]
        private float maxPitch = 80.0f;

        [Tooltip("How far in degrees can you move the camera down.")]
        [SerializeField]
        private float minPitch = -80.0f;

        [Space(15.0f)] [Tooltip("Cinemachine Virtual Camera positioned at desired crouched height.")]
        [SerializeField]
        private CinemachineVirtualCamera crouchedCamera;

        [Tooltip("Cinemachine Virtual Camera positioned at desired un-crouched height.")]
        [SerializeField]
        private CinemachineVirtualCamera unCrouchedCamera;
        
        [Tooltip("Noise amplitude gain multiplier.")]
        [SerializeField]
        private float amplitudeMultiplier = 1f;
        
        [Space(15.0f)] [Tooltip("Mouse look sensitivity")]
        [SerializeField]
        private Vector2 lookSensitivity = new Vector2(1.5f, 1.25f);
        
        private CinemachineBasicMultiChannelPerlin _unCrouchedNoiseProfile;
        
        private CinemachineBasicMultiChannelPerlin _crouchedNoiseProfile;
        
        // Cached Character

        private Character _character;

        // Current camera target pitch

        private float _cameraTargetPitch;

        private void Awake()
        {
            _character = GetComponent<Character>();
            
            if (unCrouchedCamera != null)
            {
                _unCrouchedNoiseProfile = unCrouchedCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
            
            if (crouchedCamera != null)
            {
                _crouchedNoiseProfile = crouchedCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Disable Character's rotation mode, we'll handle it here

            _character.SetRotationMode(Character.RotationMode.None);
        }

        private void OnEnable()
        {
            // Subscribe to Character events

            _character.Crouched += OnCrouched;
            _character.UnCrouched += OnUnCrouched;
        }

        private void OnDisable()
        {
            // Unsubscribe to Character events

            _character.Crouched -= OnCrouched;
            _character.UnCrouched -= OnUnCrouched;
        }

        private void Update()
        {
            // HandleMovement();
            // HandleCameraRotation();
            UpdateNoiseAmplitude();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // Read input values
        
            Vector2 moveInput = context.ReadValue<Vector2>();
        
            // Compose a movement direction vector in world space
        
            Vector3 movementDirection = Vector3.zero;
        
            movementDirection += Vector3.forward * moveInput.y;
            movementDirection += Vector3.right * moveInput.x;
        
            // If character has a camera assigned,
            // make movement direction relative to this camera view direction
        
            if (_character.camera)
            {               
                movementDirection = movementDirection.relativeTo(_character.cameraTransform);
                Debug.Log($"Camera Transform: {_character.cameraTransform.rotation}, {context.phase}");
            }
        
            // Set character's movement direction vector
        
            _character.SetMovementDirection(movementDirection);
        }
        
        // public void OnMove(InputAction.CallbackContext context)
        // {
        //     Vector2 moveInput = context.ReadValue<Vector2>();
        //
        //     // Получаем направление камеры
        //     Vector3 forward = cameraTarget.transform.forward;
        //     Vector3 right = cameraTarget.transform.right;
        //
        //     // Проекция на плоскость, убираем компонент по оси Y
        //     forward.y = 0;
        //     right.y = 0;
        //
        //     forward.Normalize();
        //     right.Normalize();
        //
        //     // Вычисляем конечное направление движения
        //     Vector3 movementDirection = forward * moveInput.y + right * moveInput.x;
        //
        //     // Задаем направление движения персонажа
        //     _character.SetMovementDirection(movementDirection);
        // }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 lookInput = context.ReadValue<Vector2>() * lookSensitivity * Time.deltaTime;
            print($"Look Input: {lookInput}, Context {context.phase}, Value from context {context.ReadValue<Vector2>()}");
            AddControlYawInput(lookInput.x);
            AddControlPitchInput(lookInput.y, minPitch, maxPitch);
        }
        

        // private void HandleCameraRotation()
        // {
        //     Vector2 lookInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //     AddControlYawInput(lookInput.x * lookSensitivity.x);
        //     AddControlPitchInput(lookInput.y * lookSensitivity.y, minPitch, maxPitch);
        // }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
                _character.Crouch();
            else if (context.canceled)
                _character.UnCrouch();
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
                _character.Jump();
            else if (context.canceled)
                _character.StopJumping();
        }

        private void UpdateNoiseAmplitude()
        {
            if (_unCrouchedNoiseProfile == null && _crouchedNoiseProfile == null)
                return;

            float currentSpeed = _character.velocity.magnitude;
            
            if (_character.IsCrouched())
            {
                float speedRatio = Mathf.Clamp01(currentSpeed / _character.maxWalkSpeed);
                _crouchedNoiseProfile.m_AmplitudeGain = speedRatio * amplitudeMultiplier;
            }
            else
            {
                float speedRatio = Mathf.Clamp01(currentSpeed / _character.maxWalkSpeedCrouched);
                _unCrouchedNoiseProfile.m_AmplitudeGain = speedRatio * amplitudeMultiplier;
            }
        }

        /// <summary>
        /// Add input (affecting Yaw).
        /// This is applied to the Character's rotation.
        /// </summary>
        private void AddControlYawInput(float value)
        {
            _character.AddYawInput(value);
        }

        /// <summary>
        /// Add input (affecting Pitch).
        /// This is applied to the cameraTarget's local rotation.
        /// </summary>
        private void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f)
        {
            if (value == 0.0f)
                return;

            _cameraTargetPitch = MathLib.ClampAngle(_cameraTargetPitch + value, minValue, maxValue);
            cameraTarget.transform.localRotation = Quaternion.Euler(-_cameraTargetPitch, 0.0f, 0.0f);
        }

        /// <summary>
        /// When character crouches, toggle Crouched / UnCrouched cameras.
        /// </summary>
        private void OnCrouched()
        {
            crouchedCamera.Priority = 11;
            unCrouchedCamera.Priority = 10;
        }

        /// <summary>
        /// When character un-crouches, toggle Crouched / UnCrouched cameras.
        /// </summary>
        private void OnUnCrouched()
        {
            crouchedCamera.Priority = 10;
            unCrouchedCamera.Priority = 11;
        }
    }
}