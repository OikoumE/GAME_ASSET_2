using Controllers;
using Interactables;

public class ReadingTabletShuttle : ReadingTabletController
{
    public override void Interact(PlayerController pC)
    {
        base.Interact(pC);
        pC.hasReadShuttleTablet = true;
    }
}