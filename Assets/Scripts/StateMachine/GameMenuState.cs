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
            gameStateMachine.playerController.SetPlayerControl(false);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            gameStateMachine.playerController.cameraToControl.enabled = false;

            Debug.Log("Enter state: " + gameStateName);
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.playerController.cameraToControl.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            gameStateMachine.playerController.SetPlayerControl(true);

            Debug.Log("Exit state: " + gameStateName);
        }
    }
}