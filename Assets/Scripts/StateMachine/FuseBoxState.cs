namespace StateMachine
{
    public class FuseBoxState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.FuseBoxState;


        public override void UpdateState(GameStateMachine gameStateMachine)
        {
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
        }
    }
}