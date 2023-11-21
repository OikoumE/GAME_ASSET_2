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
            var prevState = gameStateMachine.GetState(gameStateMachine.PrevState);
            gameStateMachine.SetState(prevState);
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