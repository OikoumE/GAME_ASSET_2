using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerController pC);
}

public enum InteractableType
{
    Elevator,
    ReadingTablet,
    CabinetDoor,
    Bolts,
    WirePuzzle
}

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public InteractableType interactableType;
    public abstract void Interact(PlayerController pC);
}