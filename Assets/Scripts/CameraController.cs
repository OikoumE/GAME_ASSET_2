using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerToControl;
    public Camera cameraToControl;
    [DoNotSerialize] public bool playerHasControl = true;
    [SerializeField] private float sensX = 100f, sensY = 100f;
    public LayerMask interactableLayerMask;

    private readonly float mouseSensMultiplier = 0.01f;

    private float xRot, yRot, mouseX, mouseY;


    public Camera PlayerCam => cameraToControl;

    private void OnDrawGizmos()
    {
        var camTransform = cameraToControl.transform;
        Gizmos.DrawRay(camTransform.position, camTransform.forward * 10);
    }


    public void InputHandler()
    {
        if (!playerHasControl) return;
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * sensX * mouseSensMultiplier;
        xRot -= mouseY * sensY * mouseSensMultiplier;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        if (playerToControl)
        {
            cameraToControl.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            playerToControl.transform.rotation = Quaternion.Euler(0, yRot, 0);
            return;
        }

        cameraToControl.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
    }
}