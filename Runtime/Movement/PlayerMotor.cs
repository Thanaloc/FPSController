using UnityEngine;

namespace FPSController
{
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private CharacterController _CharacterController;
        [SerializeField] private Camera _Camera;
        [SerializeField] private Transform _CameraHolder;
        [SerializeField] private CameraHeadBob _HeadBob;
        [SerializeField] private FPSControllerSettingsSO _Settings;

        private float _verticalVelocity;
        private Vector3 _direction;
        private Vector3 _targetHeight;
        private Vector3 _lerpedPos;
        private float _targetFOV;
        private float _defaultFOV;

        private const float CAMERA_LERP_SPEED = 10f;
        private const float FOV_LERP_SPEED = 10f;

        private void Awake()
        {
            _targetHeight = _CameraHolder.localPosition;
            _targetFOV = _Camera.fieldOfView;
            _defaultFOV = _Camera.fieldOfView;
        }

        private void Update()
        {
            ApplyTargetFOV();
            ApplyCameraHeight();
            ApplyGravity();
            ApplyMovement();
        }

        public void Move(Vector2 p_input, float p_speed)
        {
            _direction = transform.right * p_input.x + transform.forward * p_input.y;
            _direction.x *= p_speed;
            _direction.z *= p_speed;
        }

        public void SetColliderHeight(float p_height)
        {
            _CharacterController.center = new Vector3(0, p_height / 2f, 0);
            _CharacterController.height = p_height;
        }

        public void SetCameraHeight(float p_height)
        {
            _targetHeight = new Vector3(0, p_height, 0);
        }

        public void SetSprintFOV(bool p_isSprinting)
        {
            _targetFOV = p_isSprinting
                ? _defaultFOV + _defaultFOV * _Settings.SprintFOVMultiplier
                : _defaultFOV;
        }

        public bool IsGrounded()
        {
            return _CharacterController.isGrounded;
        }

        public CharacterController GetCharacterController()
        {
            return _CharacterController;
        }

        private void ApplyGravity()
        {
            if (IsGrounded() && _verticalVelocity < 0)
                _verticalVelocity = -1f;
            else
                _verticalVelocity += Physics.gravity.y * Time.deltaTime;

            _direction.y = _verticalVelocity;
        }

        private void ApplyMovement()
        {
            _CharacterController.Move(_direction * Time.deltaTime);
            _direction.x = _direction.z = 0f;
        }

        private void ApplyCameraHeight()
        {
            _lerpedPos = Vector3.Lerp(_CameraHolder.localPosition, _targetHeight, CAMERA_LERP_SPEED * Time.deltaTime);
            _CameraHolder.localPosition = new Vector3(_lerpedPos.x, _lerpedPos.y + _HeadBob.BobOffset, _lerpedPos.z);
        }

        private void ApplyTargetFOV()
        {
            _Camera.fieldOfView = Mathf.Lerp(_Camera.fieldOfView, _targetFOV, FOV_LERP_SPEED * Time.deltaTime);
        }
    }
}
