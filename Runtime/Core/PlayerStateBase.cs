namespace FPSController
{
    public abstract class PlayerStateBase : IPlayerState
    {
        protected PlayerStateDataSO _data;

        public PlayerStateDataSO Data => _data;

        public PlayerStateBase(PlayerStateDataSO p_data)
        {
            _data = p_data;
        }

        public abstract void Enter(PlayerStateMachine p_stateMachine);
        public abstract void Execute(PlayerStateMachine p_stateMachine);
        public abstract void Exit(PlayerStateMachine p_stateMachine);
    }
}
