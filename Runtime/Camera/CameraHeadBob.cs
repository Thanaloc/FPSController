using UnityEngine;

namespace FPSController
{
    public class CameraHeadBob : MonoBehaviour
    {
        [SerializeField] private CharacterController _CharacterController;
        [SerializeField] private PlayerStateMachine _StateMachine;

        private float _timer;
        private float _bobOffset;
        private float _currentFrequency;
        private float _currentAmplitude;

        public float BobOffset => _bobOffset;

        private const float LERP_SPEED = 5f;

        private void OnEnable()
        {
            _StateMachine.OnStateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            _StateMachine.OnStateChanged -= OnStateChanged;
        }

        private void Start()
        {
            OnStateChanged(_StateMachine.CurrentState);
        }

        private void Update()
        {
            if (_CharacterController.velocity.sqrMagnitude > 0.1f)
            {
                _timer += Time.deltaTime;
                _bobOffset = Mathf.Sin(_timer * _currentFrequency) * _currentAmplitude;
            }
            else
            {
                _bobOffset = Mathf.Lerp(_bobOffset, 0f, LERP_SPEED * Time.deltaTime);
                _timer = 0f;
            }
        }

        private void OnStateChanged(PlayerStateBase p_newState)
        {
            _currentFrequency = p_newState.Data.BobFrequency;
            _currentAmplitude = p_newState.Data.BobAmplitude;
        }
    }
}
