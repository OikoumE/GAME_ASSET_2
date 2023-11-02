using System;
using UnityEngine;

public class PlayerController : CameraController, IPlayer
{
    [SerializeField] private float playerSpeed = 10, movementMultiplier = 0.1f;
    // [SerializeField] private LayerMask interactableLayerMask;


    [SerializeField] private bool useGroundChecker;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float maxGroundDistance;
    private Rigidbody mRigidbody;


    // public Camera playerCam;
    // [SerializeField] private float sensX = 100f, sensY = 100f;
    // [DoNotSerialize] public bool playerHasControl = true;
    // private readonly float mouseSensMultiplier = 0.01f;
    // private float mouseX, mouseY;
    // private float yRot, xRot;

    private void Start()
    {
        playerToControl = gameObject;
        if (cameraToControl == null)
            cameraToControl = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        mRigidbody = GetComponent<Rigidbody>();
        // playerCam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        InputHandler();
        CursorLockHandler();
        // InputHandler();
        DoRay();
    }

    private void FixedUpdate()
    {
        Input.GetKeyDown(KeyCode.Space);
        if (!playerHasControl) return;
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        var verticalMovement = Input.GetAxisRaw("Vertical");
        var transforms = transform;
        var moveDirection = transforms.forward * verticalMovement + transforms.right * horizontalMovement;
        mRigidbody.AddForce(moveDirection.normalized * (playerSpeed * movementMultiplier), ForceMode.Acceleration);
        GroundChecker();
    }


    private void OnDrawGizmos()
    {
        var camTransform = cameraToControl.transform;
        Gizmos.DrawRay(camTransform.position, camTransform.forward * 10);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
    }

    private void OnValidate()
    {
        // playerCam = GetComponentInChildren<Camera>();
        GroundChecker();
    }

    public void SetPlayerControl(bool enable)
    {
        playerHasControl = enable;
    }

    private void DoRay()
    {
        if (!Input.GetMouseButtonDown(0) || !playerHasControl) return;
        var ray = cameraToControl.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, interactableLayerMask)) return;
        var objectHit = hit.transform;
        //
        var isElevator = objectHit.gameObject.TryGetComponent(out ElevatorPanel ePanel);
        // var isTablet = objectHit.gameObject.TryGetComponent(out ReadingTabletController readingTablet);
        //
        //
        // if (!(isElevator || isTablet)) return;
        // if (isElevator)
        //     ePanel.elevatorButton.Interact(this, ePanel.panelId);
        // if (isTablet)
        //     readingTablet.Interact(this);

        InteractWithObject(hit.transform.gameObject);
    }

    private void InteractWithObject(GameObject objectToInteractWith)
    {
        if (objectToInteractWith.TryGetComponent(out Interactable interactableObject))
            switch (interactableObject.interactableType)
            {
                case InteractableType.Elevator:
                    interactableObject.Interact(this);
                    break;
                case InteractableType.ReadingTablet:
                    interactableObject.Interact(this);
                    break;
                case InteractableType.CabinetDoor:
                    break;
                case InteractableType.Bolts:
                    break;
                case InteractableType.WirePuzzle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }


    private void GroundChecker()
    {
        if (!useGroundChecker) return;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down * groundCheckDistance, out var hit))
        {
            var hitObject = hit.transform.gameObject;
            var dst = Vector3.Distance(transform.position, hit.point);
            if (dst > maxGroundDistance)
            {
                var pos = transform.position;
                pos = new Vector3(pos.x, pos.y - dst, pos.z);
            }
        }
    }

    private void CursorLockHandler()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ToggleCursorLockMode();
    }

    /// <summary>
    ///     if setState is passed, defaults to CursorLockMode.Locked;
    /// </summary>
    /// <param name="setState">whether or not to toggle or set state</param>
    /// <param name="state">what state to set if setState = true</param>
    public void SetCursorLockMode(CursorLockMode state)
    {
        Cursor.lockState = state;
    }

    public void ToggleCursorLockMode()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    // private void InputHandler()
    // {
    //     if (!playerHasControl) return;
    //     mouseX = Input.GetAxisRaw("Mouse X");
    //     mouseY = Input.GetAxisRaw("Mouse Y");
    //
    //     yRot += mouseX * sensX * mouseSensMultiplier;
    //     xRot -= mouseY * sensY * mouseSensMultiplier;
    //
    //     xRot = Mathf.Clamp(xRot, -90f, 90f);
    //
    //
    //     playerCam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    //     transform.rotation = Quaternion.Euler(0, yRot, 0);
    // }
}

public interface IPlayer
{
}