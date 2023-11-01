using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private float playerSpeed = 10, movementMultiplier = 0.1f;
    [SerializeField] private float sensX = 100f, sensY = 100f;
    [SerializeField] private LayerMask interactableLayerMask;
    [DoNotSerialize] public bool playerHasControl = true;

    public Camera playerCam;

    [SerializeField] private bool useGroundChecker;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float maxGroundDistance;

    private readonly float mouseSensMultiplier = 0.01f;


    private float mouseX, mouseY;
    private Rigidbody mRigidbody;
    private float yRot, xRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mRigidbody = GetComponent<Rigidbody>();
        playerCam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        CursorLockHandler();
        InputHandler();
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
        var playerCamTransform = playerCam.transform;
        Gizmos.DrawRay(playerCamTransform.position, playerCamTransform.forward * 10);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
    }

    private void OnValidate()
    {
        playerCam = GetComponentInChildren<Camera>();
        GroundChecker();
    }

    public void SetPlayerControl(bool enabled)
    {
        playerHasControl = enabled;
    }

    private void DoRay()
    {
        RaycastHit hit;
        var ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, interactableLayerMask)) return;
        var objectHit = hit.transform;

        var isElevator = objectHit.gameObject.TryGetComponent(out ElevatorPanel ePanel);
        var isTablet = objectHit.gameObject.TryGetComponent(out ReadingTabletController readingTablet);

        if (!(isElevator || isTablet)) return;
        if (!playerHasControl || !Input.GetMouseButtonDown(0)) return;
        if (isElevator)
            ePanel.elevatorButton.Interact(ePanel.panelId, this);
        if (isTablet)
            readingTablet.Interact(0, this);
    }

    private void GroundChecker()
    {
        if (!useGroundChecker) return;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down * groundCheckDistance, out var hit))
        {
            var hitObject = hit.transform.gameObject;
            Debug.Log("hitObject: " + hitObject.name);
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

    private void InputHandler()
    {
        if (!playerHasControl) return;
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * sensX * mouseSensMultiplier;
        xRot -= mouseY * sensY * mouseSensMultiplier;

        xRot = Mathf.Clamp(xRot, -90f, 90f);


        playerCam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
}

public interface IPlayer
{
}