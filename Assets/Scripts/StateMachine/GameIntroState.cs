namespace StateMachine
{
    public class GameIntroState : GameBaseState
    {
        public new GameStateName gameStateName = GameStateName.GameIntroState;

        public override void UpdateState(GameStateMachine gameStateMachine)
        {
            var gsm = gameStateMachine;
            var pC = gsm.playerController;
            if (pC.hasReadShuttleTablet && pC.hasPickedFuse)
                gsm.SetState(gsm.fuseBoxState);
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