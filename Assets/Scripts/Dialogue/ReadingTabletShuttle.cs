using Controllers;
using Interactables;
using StateMachine;

namespace Dialogue
{
    public class ReadingTabletShuttle : ReadingTabletController
    {
        public override void Interact(
            PlayerController pC,
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio = true
        )
        {
            base.Interact(pC, audioSourceSettings, interruptAudio);
            pC.hasReadShuttleTablet = true;
        }
    }
}