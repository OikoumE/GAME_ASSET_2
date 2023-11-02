using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    public float unScrewSpeed = 10;
    public float unScrewDistance = 1f;
    public float unScrewRotationSpeed = 1f;
    public float impulseForce = 0.01f;
    public bool startTest;
    private float lerpAlpha;
    private List<BoltAnimator> screwList = new();

    private void Start()
    {
        GetAllScrew();
    }

    private void Update()
    {
        if (!startTest) return;
        startTest = false;
        foreach (var bA in screwList)
            bA.Interact();
    }

    private void OnValidate()
    {
        GetAllScrew();
        foreach (var bA in screwList)
        {
            bA.unScrewRotateSpeed = unScrewRotationSpeed;
            bA.unScrewSpeed = unScrewSpeed;
            bA.impulseForce = impulseForce;
            bA.SetStartEndPos(unScrewDistance);
        }
    }

    private void GetAllScrew()
    {
        screwList.Clear();
        screwList = GetComponentsInChildren<BoltAnimator>().ToList();
    }
}