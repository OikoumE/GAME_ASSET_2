namespace StateMachine
{
    public abstract class GameBaseState
    {
        public GameStateName gameStateName = GameStateName.BaseState;
        protected GameStateMachine mGsm;
        public abstract void UpdateState(GameStateMachine gameStateMachine);
        public abstract void EnterState(GameStateMachine gameStateMachine);
        public abstract void ExitState(GameStateMachine gameStateMachine);
    }
}