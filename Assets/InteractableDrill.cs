using Controllers;
using Interactables;

public class InteractableDrill : Interactable
{
    private void Start()
    {
    }

    protected override void Update()
    {
    }

    public override void Interact(PlayerController pC)
    {
        if (pC.hasPickedDrill) return;
        pC.hasPickedDrill = true;
        gameObject.SetActive(false);
    }
}