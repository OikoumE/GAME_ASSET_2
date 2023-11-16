using UnityEngine;

namespace StateMachine
{
    public class GameOverState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.GameOverState;

        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            Time.timeScale = 0;
            gameStateMachine.playerController.playerHasControl = false;
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
            gameStateMachine.OpenPauseMenu();
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
        }
    }
}