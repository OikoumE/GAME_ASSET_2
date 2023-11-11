using Interactables;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : CameraController, IPlayer
    {
        [SerializeField] private float playerSpeed = 10, movementMultiplier = 0.1f;
        [SerializeField] private bool useGroundChecker;
        [SerializeField] private float groundCheckDistance = 1f;
        [SerializeField] private float maxGroundDistance;
        [SerializeField] private TMP_Text crossHair;

        [DoNotSerialize] public bool
            hasPickedDrill,
            hasPickedFuse,
            hasReadShuttleTablet;

        private FlashLightController flashLightController;


        private RaycastHit hit;

        private Rigidbody mRigidbody;

        public RaycastHit Hit => hit;

        protected override void Start()
        {
            base.Start();
            playerToControl = gameObject;
            if (cameraToControl == null)
                cameraToControl = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            mRigidbody = GetComponent<Rigidbody>();
            flashLightController = GetComponentInChildren<FlashLightController>();
        }


        private void Update()
        {
            InputHandler();
            CursorLockHandler();
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
            if (hit.point != Vector3.zero)
                Gizmos.DrawCube(hit.point, Vector3.one * 0.1f);


            var camTransform = cameraToControl.transform;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * 1000);
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawRay(position, Vector3.down * groundCheckDistance);
            Gizmos.DrawSphere(position + Vector3.up * 1.75f, 0.25f);
            Gizmos.DrawSphere(position + Vector3.up, 0.25f);
            Gizmos.DrawSphere(position + Vector3.up * .25f, 0.25f);
        }

        private void OnValidate()
        {
            GroundChecker();
        }

        public PlayerController GetPlayerController()
        {
            return this;
        }

        public void SetCrossHairEnabled(bool enable)
        {
            crossHair.enabled = enable;
        }

        public void SetPlayerControl(bool enable)
        {
            playerHasControl = enable;
        }

        public void SetCrossHairOutline(Color32 color, float width = .2f)
        {
            crossHair.outlineWidth = width;
            crossHair.outlineColor = color;
        }


        private void DoRay()
        {
            if (!playerHasControl) return;
            SetCrossHairOutline(isInteractableCrossHairColor, 0);
            var ray = cameraToControl.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit, interactableLayerMask)) return;
            var isOutOfRange = IsInteractOutOfRange(transform.position, hit.point);
            var hitInteractable = hit.transform.gameObject.TryGetComponent(out Interactable interactableObject);

            if (hitInteractable)
            {
                var canBeInteractedWith = interactableObject.isInteractable;
                if (playerHasControl && canBeInteractedWith)
                    SetCrossHairOutline(isInteractableCrossHairColor);
                if (!isOutOfRange && canBeInteractedWith)
                    SetCrossHairOutline(canInteractCrossHairColor);
                if (!isOutOfRange && interactableObject.gameObject.TryGetComponent(out KitchenDoorController kDc))
                    if (!hasPickedDrill)
                        SetCrossHairOutline(isNotInteractableCrossHairColor);
            }


            if (!Input.GetMouseButtonDown(0) || !playerHasControl || !hitInteractable || isOutOfRange) return;
            interactableObject.Interact(this);
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
    }

    public interface IPlayer
    {
        public PlayerController GetPlayerController();
    }
}