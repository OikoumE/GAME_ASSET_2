using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerToControl;
    public Camera cameraToControl;
    [DoNotSerialize] public bool playerHasControl = true, freeLookCamEnabled;
    [SerializeField] private float sensX = 100f, sensY = 100f;
    public LayerMask interactableLayerMask;

    [HideInInspector] public Vector2 xRotClamp, yRotClamp;

    [SerializeField] public ClampRotation clampRotation;

    private readonly float mouseSensMultiplier = 0.01f;

    private float xRot, yRot, mouseX, mouseY;


    public Camera PlayerCam => cameraToControl;

    public void RunInputHandler()
    {
        InputHandler();
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

    [Serializable]
    public class ClampRotation
    {
        public float xMax = 90f, xMin = -90f;
        public float yMax, yMin;
    }
}