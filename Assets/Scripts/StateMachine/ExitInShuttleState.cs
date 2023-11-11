using UnityEngine;

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
            // if (mGsm) mGsm.SetState(mGsm.gameOverState);
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;

            Debug.Log("Enter state: ExitInShuttleState");
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
            Debug.Log("Exit state: ExitInShuttleState");
        }
    }
}