using System;
using Controllers;
using Interactables;

public class InteractableFuse : Interactable
{
    private InteractableFuse[] fuses;

    private void Start()
    {
        fuses = FindObjectsOfType<InteractableFuse>();
    }


    public override void Interact(PlayerController pC)
    {
        if (pC.hasPickedFuse) return;
        pC.hasPickedFuse = true;
        gameObject.SetActive(false);
        foreach (var interactableFuse in fuses)
        {
            if (interactableFuse == this) continue;
            interactableFuse.isInteractable = false;
        }
    }

    public override void Interact(KitchenDoorController kDC)
    {
        throw new NotImplementedException();
    }

    public override void Interact(FuseboxController fC)
    {
        throw new NotImplementedException();
    }
}