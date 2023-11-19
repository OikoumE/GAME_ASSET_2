namespace StateMachine
{
    public class GameOverState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.GameOverState;

        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.playerController.playerHasControl = false;
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
            gameStateMachine.mainMenuHandler.StartGame(true);
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
        }
    }
}