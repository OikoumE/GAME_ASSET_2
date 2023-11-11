using Controllers;
using Interactables;

namespace Dialogue
{
    public class ReadingTabletShuttle : ReadingTabletController
    {
        public override void Interact(PlayerController pC)
        {
            base.Interact(pC);
            pC.hasReadShuttleTablet = true;
        }
    }
}