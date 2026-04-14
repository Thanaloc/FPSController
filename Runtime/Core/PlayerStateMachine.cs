using System;
using UnityEngine;

namespace FPSController
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMotor _Motor;
        [SerializeField] private PlayerInputHandler _InputHandler;

        [Header("State Data")]
        [SerializeField] private PlayerStateDataSO _IdleData;
        [SerializeField] private PlayerStateDataSO _WalkData;
        [SerializeField] private PlayerStateDataSO _CrouchData;
        [SerializeField] private PlayerStateDataSO _SprintData;

        public PlayerMotor Motor => _Motor;
        public PlayerInputHandler Input => _InputHandler;

        public PlayerStateBase IdleState => _idleState;
        public PlayerStateBase WalkState => _walkState;
        public PlayerStateBase CrouchState => _crouchState;
        public PlayerStateBase SprintState => _sprintState;
        public PlayerStateBase CurrentState => _currentState;

        public Action<PlayerStateBase> OnStateChanged;

        private PlayerStateBase _currentState;
        private PlayerStateBase _idleState;
        private PlayerStateBase _walkState;
        private PlayerStateBase _crouchState;
        private PlayerStateBase _sprintState;

        private void Awake()
        {
            _idleState = new PlayerIdleState(_IdleData);
            _walkState = new PlayerWalkState(_WalkData);
            _crouchState = new PlayerCrouchState(_CrouchData);
            _sprintState = new PlayerSprintState(_SprintData);

            _currentState = _idleState;
            _currentState.Enter(this);
        }

        /// <summary>
        /// Emitted in Start so all subscribers (who register in OnEnable) are guaranteed to receive it.
        /// </summary>
        private void Start()
        {
            OnStateChanged?.Invoke(_currentState);
        }

        private void Update()
        {
            _currentState.Execute(this);
        }

        public void TransitionTo(PlayerStateBase p_newState)
        {
            if (p_newState == _currentState)
                return;

            _currentState.Exit(this);
            _currentState = p_newState;
            _currentState.Enter(this);
            OnStateChanged?.Invoke(_currentState);
        }
    }
}