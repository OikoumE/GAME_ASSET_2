using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DockingLampController : MonoBehaviour
{
    public Light leftLight, rightLight;
    public MeshRenderer leftLED, rightLED;
    public Material greenLED, redLED;
    public DockingSide dockingSide;

    [SerializeField] private List<GameObject> dockingDoors;
    public UnityEvent[] doors;


    private void Start()
    {
        SetDockingStatus();
    }

    private void OnValidate()
    {
        SetDockingStatus();
    }


    private void SetDockingStatus()
    {
        if (leftLight == null || rightLight == null || leftLED == null || rightLED == null)
            throw new Exception("Missing component for: left, right, leftLED or rightLED");

        Color leftColor;
        Color rightColor;
        Material leftMaterial;
        Material rightMaterial;
        switch (dockingSide)
        {
            case DockingSide.Left:
                leftColor = Color.green;
                rightColor = Color.red;
                leftMaterial = greenLED;
                rightMaterial = redLED;
                break;
            case DockingSide.Right:
                leftColor = Color.red;
                rightColor = Color.green;
                leftMaterial = greenLED;
                rightMaterial = redLED;
                break;
            case DockingSide.None:
                leftColor = Color.red;
                rightColor = Color.red;
                leftMaterial = redLED;
                rightMaterial = redLED;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        leftLight.color = leftColor;
        rightLight.color = rightColor;
        leftLED.material = leftMaterial;
        rightLED.material = rightMaterial;
    }
}

public enum DockingSide
{
    None,
    Left,
    Right
}