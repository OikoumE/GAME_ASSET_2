using System;
using Interactables;
using UnityEngine;

namespace Controllers
{
    public class DockingLampController : MonoBehaviour
    {
        public Light leftLight, rightLight;
        public MeshRenderer leftLED, rightLED;
        public Material greenLED, redLED;
        public DockingSide dockingSide;

        [SerializeField] private OpenDoor leftDoor;
        [SerializeField] private OpenDoor rightDoor;


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
            bool leftCanOpen, rightCanOpen;
            switch (dockingSide)
            {
                case DockingSide.Left:
                    leftColor = Color.green;
                    rightColor = Color.red;
                    leftMaterial = greenLED;
                    rightMaterial = redLED;
                    leftCanOpen = true;
                    rightCanOpen = false;
                    break;
                case DockingSide.Right:
                    leftColor = Color.red;
                    rightColor = Color.green;
                    leftMaterial = redLED;
                    rightMaterial = greenLED;
                    leftCanOpen = false;
                    rightCanOpen = true;
                    break;
                case DockingSide.None:
                    leftColor = Color.red;
                    rightColor = Color.red;
                    leftMaterial = redLED;
                    rightMaterial = redLED;
                    leftCanOpen = false;
                    rightCanOpen = false;
                    break;
                case DockingSide.Both:
                    leftColor = Color.green;
                    rightColor = Color.green;
                    leftMaterial = greenLED;
                    rightMaterial = greenLED;
                    leftCanOpen = true;
                    rightCanOpen = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            leftLight.color = leftColor;
            rightLight.color = rightColor;
            leftLED.material = leftMaterial;
            rightLED.material = rightMaterial;
            leftDoor.canBeOpened = leftCanOpen;
            rightDoor.canBeOpened = rightCanOpen;
        }
    }

    public enum DockingSide
    {
        None,
        Left,
        Right,
        Both
    }
}