using System;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public GameObject playerToControl;
        public Camera cameraToControl;
        public bool playerHasControl = true;
        [SerializeField] private float interactDistanceThreshold = 1f;
        [SerializeField] private float sensX = 100f, sensY = 100f;
        public LayerMask interactableLayerMask, worldLayerMask;

        [SerializeField] protected Color32
            defaultCrossHairColor = new(0, 0, 0, 255),
            canInteractCrossHairColor = new(0, 255, 0, 255),
            isInteractableCrossHairColor = new(255, 255, 255, 255),
            isNotInteractableCrossHairColor = new(255, 0, 0, 255);

        [SerializeField] public ClampRotation clampRotation;
        [SerializeField] private float initialWaitBeforeControl;

        private readonly float mouseSensMultiplier = 0.01f;
        private bool hasWaitedForControl;
        private float xRot, yRot, mouseX, mouseY;


        public Camera PlayerCam => cameraToControl;
        public Color32 IsInteractableCrossHairColor => isInteractableCrossHairColor;
        public Color32 CanInteractCrossHairColor => canInteractCrossHairColor;

        protected virtual void Start()
        {
            var rot = transform.localRotation.eulerAngles;
            yRot = rot.y;
        }


        private void OnDrawGizmos()
        {
            var tr = transform;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(tr.position, tr.forward * 10);
        }


        public void RunInputHandler()
        {
            InputHandler();
        }

        protected bool IsInteractOutOfRange(Vector3 pointA, Vector3 pointB)
        {
            return Vector3.Distance(pointA, pointB) > interactDistanceThreshold;
        }

        protected void InputHandler()
        {
            if (!playerHasControl) return;
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");
            yRot += mouseX * sensX * mouseSensMultiplier;
            xRot -= mouseY * sensY * mouseSensMultiplier;
            xRot = Mathf.Clamp(xRot, clampRotation.xMin, clampRotation.xMax);

            if (playerToControl)
            {
                cameraToControl.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
                playerToControl.transform.rotation = Quaternion.Euler(0, yRot, 0);
                return;
            }

            yRot = Mathf.Clamp(yRot, clampRotation.yMin, clampRotation.yMax);
            cameraToControl.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
        }

        public void SetStartRot(Vector2 rot)
        {
            xRot = rot.x;
            yRot = rot.y;
        }

        public void SetClampRotation(ClampRotation clampRot)
        {
            clampRotation = clampRot;
        }

        [Serializable]
        public class ClampRotation
        {
            public float xMax = 90f, xMin = -90f;
            public float yMax, yMin;
        }
    }
}