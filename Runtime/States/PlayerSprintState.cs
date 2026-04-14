namespace FPSController
{
    public class PlayerSprintState : PlayerStateBase
    {
        public PlayerSprintState(PlayerStateDataSO p_data) : base(p_data) { }

        public override void Enter(PlayerStateMachine p_stateMachine)
        {
            p_stateMachine.Motor.SetSprintFOV(true);
            p_stateMachine.Motor.SetColliderHeight(_data.ColliderHeight);
            p_stateMachine.Motor.SetCameraHeight(_data.CameraHeight);
            p_stateMachine.Motor.SetMovementSmoothing(_data.Acceleration, _data.Deceleration);
        }

        public override void Execute(PlayerStateMachine p_stateMachine)
        {
            bool hasInput = p_stateMachine.Input.MoveInput.sqrMagnitude > 0.1f;
            bool movingForward = p_stateMachine.Input.MoveInput.y > 0.5f;

            // Exit sprint if no longer moving forward
            if (!movingForward || !p_stateMachine.Input.SprintPressed)
            {
                if (p_stateMachine.Input.CrouchPressed)
                {
                    p_stateMachine.TransitionTo(p_stateMachine.CrouchState);
                    return;
                }

                if (hasInput)
                {
                    p_stateMachine.TransitionTo(p_stateMachine.WalkState);
                    return;
                }

                p_stateMachine.TransitionTo(p_stateMachine.IdleState);
                return;
            }

            p_stateMachine.Motor.Move(p_stateMachine.Input.MoveInput, _data.MoveSpeed);
        }

        public override void Exit(PlayerStateMachine p_stateMachine)
        {
            p_stateMachine.Motor.SetSprintFOV(false);
        }
    }
}