using UnityEngine;

namespace StateMachine
{
    public class GameMenuState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.GameMenuState;

        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            //TODO something
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
            // Time.timeScale = 0;
            gameStateMachine.playerController.cameraToControl.enabled = false;
            gameStateMachine.playerController.SetPlayerControl(false);
            gameStateMachine.playerController.SetCrossHairEnabled(false);
            gameStateMachine.SetCursorLockMode(CursorLockMode.None);
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
            Time.timeScale = 1;
            gameStateMachine.playerController.SetPlayerControl(true);
            gameStateMachine.playerController.SetCrossHairEnabled(true);
        }
    }
}