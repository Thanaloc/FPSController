using System;
using UnityEngine;

namespace FPSController
{
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private CharacterController _CharacterController;
        [SerializeField] private Camera _Camera;
        [SerializeField] private Transform _CameraHolder;
        [SerializeField] private CameraHeadBob _HeadBob;
        [SerializeField] private CameraLandingImpact _LandingImpact;
        [SerializeField] private FPSControllerSettingsSO _Settings;
        [SerializeField] private PlayerInputHandler _InputHandler;

        private float _verticalVelocity;
        private Vector3 _targetHeight;
        private Vector3 _lerpedPos;
        private float _targetFOV;
        private float _defaultFOV;

        // Smoothed horizontal movement
        private Vector3 _currentHorizontalVelocity;
        private Vector3 _targetHorizontalVelocity;
        private float _currentAcceleration = 50f;
        private float _currentDeceleration = 40f;

        // Ground check (single source of truth)
        private bool _isGrounded;
        private bool _wasGroundedLastFrame;

        // Coyote time & jump buffer
        private float _coyoteTimer;
        private float _jumpBufferTimer;

        // Landing detection
        private float _previousVerticalVelocity;

        private const float CAMERA_LERP_SPEED = 10f;
        private const float FOV_LERP_SPEED = 10f;
        private const float GROUND_CHECK_DISTANCE = 0.3f;
        private const float GROUND_CHECK_OFFSET = 0.1f;

        private bool _gravityOverride;
        private bool _movementEnabled = true;
        private bool _jumpEnabled = true;

        /// <summary>
        /// True if the player is on the ground. Single source of truth (SphereCast).
        /// </summary>
        public bool IsGrounded => _isGrounded;

        /// <summary>
        /// Vertical velocity captured the frame before the last landing.
        /// Used by CameraLandingImpact to scale the camera dip.
        /// </summary>
        public float LastLandingVelocity { get; private set; }

        /// <summary>
        /// Fired the frame the player touches the ground after being airborne.
        /// Parameter is the vertical velocity at impact (negative value).
        /// </summary>
        public event Action<float> OnLanded;

        private void Awake()
        {
            _targetHeight = _CameraHolder.localPosition;
            _targetFOV = _Camera.fieldOfView;
            _defaultFOV = _Camera.fieldOfView;
        }

        private void Update()
        {
            UpdateGroundCheck();
            UpdateTimers();
            ApplyTargetFOV();
            ApplyGravity();
            ApplySmoothedMovement();
            ApplyCameraHeight();
        }

        //Ground Check

        private void UpdateGroundCheck()
        {
            _wasGroundedLastFrame = _isGrounded;

            float radius = _CharacterController.radius * 0.9f;
            Vector3 origin = transform.position + Vector3.up * (radius + GROUND_CHECK_OFFSET);

            if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, GROUND_CHECK_OFFSET + GROUND_CHECK_DISTANCE))
                _isGrounded = true;
            else
                _isGrounded = false;

            // Landing event
            if (_isGrounded && !_wasGroundedLastFrame)
            {
                LastLandingVelocity = _previousVerticalVelocity;
                OnLanded?.Invoke(_previousVerticalVelocity);
            }
        }

        //Timers

        private void UpdateTimers()
        {
            if (_isGrounded)
                _coyoteTimer = _Settings.CoyoteTime;
            else
                _coyoteTimer -= Time.deltaTime;

            if (_InputHandler.JumpPressed)
                _jumpBufferTimer = _Settings.JumpBufferTime;
            else
                _jumpBufferTimer -= Time.deltaTime;
        }

        //Public API

        public void Move(Vector2 p_input, float p_speed)
        {
            if (!_movementEnabled)
            {
                _targetHorizontalVelocity = Vector3.zero;
                return;
            }

            Vector3 direction = transform.right * p_input.x + transform.forward * p_input.y;

            float speedMultiplier = _isGrounded ? 1f : _Settings.AirControlMultiplier;

            _targetHorizontalVelocity = direction * (p_speed * speedMultiplier);
        }

        public void SetMovementSmoothing(float p_acceleration, float p_deceleration)
        {
            _currentAcceleration = p_acceleration;
            _currentDeceleration = p_deceleration;
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

        public CharacterController GetCharacterController()
        {
            return _CharacterController;
        }

        public void SetHeadBobEnabled(bool p_enabled)
        {
            _HeadBob.enabled = p_enabled;
        }

        /// <summary>
        /// When true, the motor skips all gravity and jump processing.
        /// Use SetVerticalVelocity() to control vertical movement manually
        /// (ladders, swimming, cutscenes, etc.).
        /// </summary>
        public void SetGravityOverride(bool p_override)
        {
            _gravityOverride = p_override;
            if (p_override) _verticalVelocity = 0f;
        }

        public void SetVerticalVelocity(float p_velocity)
        {
            _verticalVelocity = p_velocity;
        }

        public void SetMovementEnabled(bool p_enabled)
        {
            _movementEnabled = p_enabled;
            if (!p_enabled)
                _currentHorizontalVelocity = Vector3.zero;
        }

        public void SetJumpEnabled(bool p_enabled)
        {
            _jumpEnabled = p_enabled;
        }

        //Gravity & Jump

        private void ApplyGravity()
        {
            _previousVerticalVelocity = _verticalVelocity;

            if (_gravityOverride)
                return;

            if (_isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -1f;
            }
            else
            {
                float gravityScale = 1f;

                if (_verticalVelocity < 0f)
                    gravityScale = _Settings.FallGravityMultiplier;
                else if (_verticalVelocity > 0f && !_InputHandler.JumpPressed)
                    gravityScale = _Settings.LowJumpGravityMultiplier;

                _verticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
            }

            bool canJump = _jumpEnabled && _jumpBufferTimer > 0f && _coyoteTimer > 0f;
            if (canJump)
            {
                _verticalVelocity = _Settings.JumpForce;
                _coyoteTimer = 0f;
                _jumpBufferTimer = 0f;
                _InputHandler.ConsumeJump();
            }
        }

        //Movement

        private void ApplySmoothedMovement()
        {
            if (!_CharacterController.enabled)
                return;

            // Pick accel or decel rate based on whether we're speeding up or slowing down
            float rate = _targetHorizontalVelocity.sqrMagnitude >= _currentHorizontalVelocity.sqrMagnitude
                ? _currentAcceleration
                : _currentDeceleration;

            _currentHorizontalVelocity = Vector3.MoveTowards(
                _currentHorizontalVelocity,
                _targetHorizontalVelocity,
                rate * Time.deltaTime
            );

            Vector3 finalMove = new Vector3(
                _currentHorizontalVelocity.x,
                _verticalVelocity,
                _currentHorizontalVelocity.z
            );

            _CharacterController.Move(finalMove * Time.deltaTime);

            // Reset target: if no state calls Move() next frame, we decelerate to zero
            _targetHorizontalVelocity = Vector3.zero;
        }

        //Camera

        private void ApplyCameraHeight()
        {
            _lerpedPos = Vector3.Lerp(
                _CameraHolder.localPosition,
                _targetHeight,
                CAMERA_LERP_SPEED * Time.deltaTime
            );

            float additionalOffset = _HeadBob.BobOffset;
            if (_LandingImpact != null)
                additionalOffset += _LandingImpact.ImpactOffset;

            _CameraHolder.localPosition = new Vector3(
                _lerpedPos.x,
                _lerpedPos.y + additionalOffset,
                _lerpedPos.z
            );
        }

        private void ApplyTargetFOV()
        {
            _Camera.fieldOfView = Mathf.Lerp(
                _Camera.fieldOfView,
                _targetFOV,
                FOV_LERP_SPEED * Time.deltaTime
            );
        }

    }
}
