using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerController pC, int CamID);

    public void Interact(PlayerController pC);
}

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public enum InteractableType
    {
        Elevator,
        ReadingTablet,
        CabinetDoor,
        Bolts,
        WirePuzzle
    }

    public InteractableType interactableType;

    public abstract void Interact(PlayerController pC, int CamID);
    public abstract void Interact(PlayerController pC);
}