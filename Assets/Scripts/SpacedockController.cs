using System.Collections.Generic;
using UnityEngine;

public class SpacedockController : MonoBehaviour
{
    public float doorOpenSpeed;
    [SerializeField] [Range(0, 100)] private float allowThroughDoorThreshold;
    [SerializeField] private List<OpenDoor> doorsThatCanOpen;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        GetAllDoors();
    }

    private void Update()
    {
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        foreach (var door in doorsThatCanOpen)
            door.OpenAnimation();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        foreach (var door in doorsThatCanOpen)
            door.CloseAnimation();
    }


    private void GetAllDoors()
    {
        doorsThatCanOpen.Clear();
        var allDoors = GetComponentsInChildren<OpenDoor>();
        foreach (var door in allDoors)
            if (door.canBeOpened && door.doorPosition == OpenDoor.DoorPosition.middle)
            {
                doorsThatCanOpen.Add(door);
                door.SetTriggerBoxActive(false);
                door.OpenSpeed = doorOpenSpeed;
                door.AllowThroughDoorThreshold = allowThroughDoorThreshold;
            }
    }
}