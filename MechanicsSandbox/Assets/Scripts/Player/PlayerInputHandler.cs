using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Input Action Asset")]
        [SerializeField] private InputActionAsset playerControls;

        [Header("Action Map Name References")]
        [SerializeField] private string actionMapName = "Player";
    
        [Header("Action Name References")]
        [SerializeField] private string move = "Move";
        [SerializeField] private string look = "Look";
        [SerializeField] private string jump = "Jump";
        [SerializeField] private string crouch = "Crouch";
        [SerializeField] private string interact = "Interact";
        [SerializeField] private string shoot = "Shoot";

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _crouchAction;
        private InputAction _interactAction;
        private InputAction _shootAction;
    
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpTriggered { get; private set; }
        public bool CrouchTriggered { get; private set; }
        public bool InteractTriggered { get; private set; }
        public bool ShootTriggered { get; private set; }
        
    
        public static PlayerInputHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
            _lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
            _jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
            _crouchAction = playerControls.FindActionMap(actionMapName).FindAction(crouch);
            _interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);
            _shootAction = playerControls.FindActionMap(actionMapName).FindAction(shoot);
            RegisterInputActions();
        }

        private void RegisterInputActions()
        {
            _moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
            _moveAction.canceled += context => MoveInput = Vector2.zero;

            _lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
            _lookAction.canceled += context => LookInput = Vector2.zero;

            _jumpAction.performed += context => JumpTriggered = true;
            _jumpAction.canceled += context => JumpTriggered = false;

            _crouchAction.performed += context => CrouchTriggered = true;
            _crouchAction.canceled += context => CrouchTriggered = false;

            _interactAction.performed += context => InteractTriggered = true;
            _interactAction.canceled += context => InteractTriggered = false;
            
            _shootAction.performed += context => ShootTriggered = true;
            _shootAction.canceled += context => ShootTriggered = false;
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _lookAction.Enable();
            _jumpAction.Enable();
            _crouchAction.Enable();
            _interactAction.Enable();
            _shootAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _lookAction.Disable();
            _jumpAction.Disable();
            _crouchAction.Disable();
            _interactAction.Disable();
            _shootAction.Disable();
        }
    }
}
