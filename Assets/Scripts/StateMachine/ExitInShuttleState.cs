namespace StateMachine
{
    public class ExitInShuttleState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.ExitInShuttleState;

        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            mGsm = gameStateMachine;


            //TODO condition
            //todo make trigger for gameOverState (in shuttle, by seat)
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
            gameStateMachine.ductTapeDialogueTrigger.DuctTapeSolution();
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
        }
    }
}