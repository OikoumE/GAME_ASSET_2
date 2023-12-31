namespace StateMachine
{
    public class WireGameState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.WireGameState;


        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            mGsm = gameStateMachine;
        }

        public override void EnterState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.currentStateName = gameStateName;
            gameStateMachine.lightController.SetAllLightsEnabled(false);
            gameStateMachine.wireGameAudioController.SetParticlePlayState(true);
        }

        public override void ExitState(GameStateMachine gameStateMachine)
        {
            gameStateMachine.lightController.SetAllLightsEnabled(true);
            gameStateMachine.wireGameAudioController.SetParticlePlayState(false);
        }
    }
}