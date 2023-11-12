using Controllers;
using Elevator;
using Interactables;

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