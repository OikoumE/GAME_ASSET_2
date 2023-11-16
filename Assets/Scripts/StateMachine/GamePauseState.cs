using UnityEngine;

namespace StateMachine
{
    public class GamePauseState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.FuseBoxState;


        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            gameStateMachine.ToggleCursorLockMode();
            gameStateMachine.SetState(gameStateMachine.PrevState);
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