using System.Collections.Generic;
using System.Linq;
using Interactables;
using UnityEngine;

namespace Controllers
{
    public class BoltController : MonoBehaviour
    {
        public float unScrewSpeed = 10;
        public float unScrewDistance = 1f;
        public float unScrewRotationSpeed = 1f;
        public float impulseForce = 0.01f;
        public bool startTest;
        public float drillAnimDuration = 3f;

        public int unScrewedScrews;
        private List<BoltAnimator> screwList = new();

        // private void Start()
        // {
        //     GetAllScrew();
        // }

        // private void Update()
        // {
        //     if (!startTest) return;
        //     startTest = false;
        //     foreach (var bA in screwList)
        //         bA.Interact();
        // }

        private void OnValidate()
        {
            GetAllScrew();
            foreach (var bA in screwList)
            {
                bA.unScrewRotateSpeed = unScrewRotationSpeed;
                bA.lerpSpeed = unScrewSpeed;
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
}