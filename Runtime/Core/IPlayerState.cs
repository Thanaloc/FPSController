namespace FPSController
{
    public interface IPlayerState
    {
        void Enter(PlayerStateMachine p_stateMachine);
        void Execute(PlayerStateMachine p_stateMachine);
        void Exit(PlayerStateMachine p_stateMachine);
    }
}
