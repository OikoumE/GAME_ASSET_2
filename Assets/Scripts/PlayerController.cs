using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private float playerSpeed = 10, movementMultiplier = 0.1f;
    [SerializeField] private float sensX = 100f, sensY = 100f;

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
        playerCam = GetComponentInChildren<Camera>();
        mRigidbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CursorLockHandler();
        InputHandler();
        DoRay();
    }

    private void FixedUpdate()
    {
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

        if (!Physics.Raycast(ray, out hit)) return;
        var objectHit = hit.transform;
        if (!objectHit.gameObject.TryGetComponent(out ElevatorButton eB)) return;
        if (Input.GetMouseButtonDown(0)) eB.Interact(eB.interactibleID, this);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
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