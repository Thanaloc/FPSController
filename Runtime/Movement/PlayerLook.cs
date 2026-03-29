using UnityEngine;

namespace FPSController
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler _InputHandler;
        [SerializeField] private Transform _CameraHolder;
        [SerializeField, Range(0f, 50f)] private float _MouseSensitivity = 1f;

        private float _verticalRotation;
        private Vector2 _lookInput;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _lookInput = _InputHandler.LookInput * _MouseSensitivity;

            transform.Rotate(Vector3.up, _lookInput.x);

            _verticalRotation += _lookInput.y;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);

            _CameraHolder.localRotation = Quaternion.Euler(-_verticalRotation, 0f, 0f);
        }
    }
}
