using UnityEngine;

namespace StateMachine
{
    public class GameOverState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.GameOverState;

        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            //TODO something
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
            Debug.Log("Enter state: " + gameStateName);
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
            Debug.Log("Exit state: " + gameStateName);
        }
    }
}