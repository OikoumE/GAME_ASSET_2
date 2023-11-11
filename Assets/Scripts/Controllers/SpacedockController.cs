using System.Collections.Generic;
using Controllers;
using Interactables;
using UnityEngine;

public class SpacedockController : MonoBehaviour
{
    public float doorOpenSpeed;
    [SerializeField] [Range(0, 100)] private float allowThroughDoorThreshold;
    [SerializeField] private bool refreshDoorSettings;
    [SerializeField] private List<OpenDoor> doorsThatCanOpen;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        SetAllDoorsSettings();
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

    private void OnValidate()
    {
        if (refreshDoorSettings) refreshDoorSettings = false;
        SetAllDoorsSettings();
    }


    private void SetAllDoorsSettings()
    {
        doorsThatCanOpen.Clear();
        var allDoors = GetComponentsInChildren<OpenDoor>();
        foreach (var door in allDoors)
        {
            if (door.canBeOpened && door.doorPosition == OpenDoor.DoorPosition.middle)
            {
                doorsThatCanOpen.Add(door);
                door.SetTriggerBoxActive(false);
            }

            door.OpenSpeed = doorOpenSpeed;
            door.AllowThroughDoorThreshold = allowThroughDoorThreshold;
        }
    }
}