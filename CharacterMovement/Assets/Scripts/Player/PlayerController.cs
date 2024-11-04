using Cinemachine;
using ECM2;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
        
        [Space(15.0f)] [Tooltip("Mouse look sensitivity")]
        [SerializeField]
        private Vector2 mouseSensitivity = new Vector2(1.5f, 1.25f);

        [Space(15.0f)] [Tooltip("Cinemachine Virtual Camera positioned at desired crouched height.")]
        [SerializeField]
        private CinemachineVirtualCamera crouchedCamera;
        
        [Tooltip("Cinemachine Virtual Camera positioned at desired un-crouched height.")]
        [SerializeField]
        private CinemachineVirtualCamera unCrouchedCamera;
        
        [Tooltip("Noise amplitude gain multiplier.")]
        [SerializeField]
        private float amplitudeMultiplier = 1f;
        
        private CinemachineBasicMultiChannelPerlin _unCrouchedNoiseProfile;
        private CinemachineBasicMultiChannelPerlin _crouchedNoiseProfile;
        private Character _character;
        private float _cameraTargetPitch;
        private PlayerInputHandler _inputHandler;

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
            
            _inputHandler = PlayerInputHandler.Instance;
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
            HandleMovement();
            HandleRotation();
            UpdateNoiseAmplitude();
        }

        private void HandleMovement()
        {
            Vector3 inputDirection = new Vector3(_inputHandler.MoveInput.x, 0f, _inputHandler.MoveInput.y);
            Vector3 worldDirection = transform.TransformDirection(inputDirection);
            worldDirection.Normalize();

            HandleJumping();
            HandleCrouching();
            _character.SetMovementDirection(worldDirection);
        }

        private void HandleJumping()
        {
            if (_inputHandler.JumpTriggered)
            {
                _character.Jump();
            }
            else
            {
                _character.StopJumping();
            }
        }

        private void HandleCrouching()
        {
            if (_inputHandler.CrouchTriggered)
            {
                _character.Crouch();
            }
            else
            {
                _character.UnCrouch();
            }
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
        
        private void HandleRotation()
        {
            float mouseXRotation = _inputHandler.LookInput.x * mouseSensitivity.x;
            AddControlYawInput(mouseXRotation);

            float mouseYRotation = _inputHandler.LookInput.y * mouseSensitivity.y;
            AddControlPitchInput(mouseYRotation, minPitch, maxPitch);
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
    }
}