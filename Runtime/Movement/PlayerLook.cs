using UnityEngine;

namespace FPSController
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler _InputHandler;
        [SerializeField] private Transform _CameraHolder;
        [SerializeField, Range(0f, 50f)] private float _MouseSensitivity = 1f;

        [Header("Smoothing (optional)")]
        [SerializeField] private bool _EnableSmoothing = false;
        [SerializeField, Range(0.01f, 0.1f)] private float _SmoothTime = 0.03f;

        private float _verticalRotation;
        private Vector2 _lookInput;
        private Vector2 _smoothedInput;
        private Vector2 _smoothVelocity;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            Vector2 rawInput = _InputHandler.LookInput * _MouseSensitivity;

            if (_EnableSmoothing)
            {
                _smoothedInput.x = Mathf.SmoothDamp(_smoothedInput.x, rawInput.x, ref _smoothVelocity.x, _SmoothTime);
                _smoothedInput.y = Mathf.SmoothDamp(_smoothedInput.y, rawInput.y, ref _smoothVelocity.y, _SmoothTime);
                _lookInput = _smoothedInput;
            }
            else
            {
                _lookInput = rawInput;
            }

            transform.Rotate(Vector3.up, _lookInput.x);

            _verticalRotation += _lookInput.y;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);

            _CameraHolder.localRotation = Quaternion.Euler(-_verticalRotation, 0f, 0f);
        }
    }
}