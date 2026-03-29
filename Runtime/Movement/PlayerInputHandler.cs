using UnityEngine;
using UnityEngine.InputSystem;

namespace FPSController
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Action References")]
        [SerializeField] private InputActionReference _MoveAction;
        [SerializeField] private InputActionReference _LookAction;
        [SerializeField] private InputActionReference _SprintAction;
        [SerializeField] private InputActionReference _CrouchAction;
        [SerializeField] private InputActionReference _JumpAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool SprintPressed { get; private set; }
        public bool CrouchPressed { get; private set; }
        public bool JumpPressed { get; private set; }

        private void OnEnable()
        {
            _MoveAction.action.Enable();
            _LookAction.action.Enable();
            _SprintAction.action.Enable();
            _CrouchAction.action.Enable();
            _JumpAction.action.Enable();

            _MoveAction.action.performed += OnMove;
            _MoveAction.action.canceled += OnMove;
            _LookAction.action.performed += OnLook;
            _LookAction.action.canceled += OnLook;
            _SprintAction.action.performed += OnSprint;
            _SprintAction.action.canceled += OnSprint;
            _CrouchAction.action.performed += OnCrouch;
            _CrouchAction.action.canceled += OnCrouch;
            _JumpAction.action.performed += OnJump;
            _JumpAction.action.canceled += OnJump;
        }

        private void OnDisable()
        {
            _MoveAction.action.performed -= OnMove;
            _MoveAction.action.canceled -= OnMove;
            _LookAction.action.performed -= OnLook;
            _LookAction.action.canceled -= OnLook;
            _SprintAction.action.performed -= OnSprint;
            _SprintAction.action.canceled -= OnSprint;
            _CrouchAction.action.performed -= OnCrouch;
            _CrouchAction.action.canceled -= OnCrouch;
            _JumpAction.action.performed -= OnJump;
            _JumpAction.action.canceled -= OnJump;

            _MoveAction.action.Disable();
            _LookAction.action.Disable();
            _SprintAction.action.Disable();
            _CrouchAction.action.Disable();
            _JumpAction.action.Disable();
        }

        private void OnMove(InputAction.CallbackContext p_context)
        {
            MoveInput = p_context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext p_context)
        {
            LookInput = p_context.ReadValue<Vector2>();
        }

        private void OnSprint(InputAction.CallbackContext p_context)
        {
            SprintPressed = p_context.ReadValueAsButton();
        }

        private void OnCrouch(InputAction.CallbackContext p_context)
        {
            CrouchPressed = p_context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext p_context)
        {
            JumpPressed = p_context.ReadValueAsButton();
        }
    }
}
