using System.Collections.Generic;
using UnityEngine;

public class SpaceDockHandler : MonoBehaviour
{
    [SerializeField] private List<OpenDoor> middleDoors;
    [SerializeField] private OpenDoor spaceStationDoor;

    [SerializeField] private bool test;
    [SerializeField] private float openSpeed = 10f;

    private float blendAlpha;


    private void Start()
    {
        //TODO implement staggered animation... 
        // disable each middle doors OpenDoor script?
        GetMiddleDoors();
    }

    private void Update()
    {
        // TriggerStaggeredAnimation();
        TriggerAnimation();
    }

    private void OnValidate()
    {
        // GetMiddleDoors();
    }

    private void GetMiddleDoors()
    {
        middleDoors.Clear();
        middleDoors.Add(spaceStationDoor);
        var allDoors = GetComponentsInChildren<OpenDoor>();
        foreach (var door in allDoors)
            if (door.doorPosition == OpenDoor.DoorPosition.middle && door.canBeOpened)
                middleDoors.Add(door);
    }

    private void TriggerStaggeredAnimation()
    {
        if (test)
        {
            middleDoors[0].SetAnimate(true);
            test = false;
        }

        for (var i = 0; i < middleDoors.Count; i++)
        {
            var alpha = middleDoors[i].GetAlpha();
            if (alpha is <= 25 or >= 30) continue;
            if (middleDoors[i + 1].GetAnimate())
                middleDoors[i + 1].SetAnimate(true);
            break;
        }
    }

    private void TriggerAnimation()
    {
        if (!test) return;
        if (blendAlpha < 100)
            blendAlpha += Time.deltaTime * openSpeed;
        for (var i = 0; i < middleDoors.Count; i++)
            middleDoors[i].SetBlendShape(blendAlpha);
    }
}