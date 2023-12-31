using Interactables;
using StateMachine;
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


        [SerializeField] private AudioSourceSettings lerpAudioSourceSettings;

        [DoNotSerialize] public bool hasPickedDrill;
        [DoNotSerialize] public bool hasPickedFuse;
        [DoNotSerialize] public bool hasReadShuttleTablet;
        [DoNotSerialize] public bool hasFlashLight;

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
            // Cursor.lockState = CursorLockMode.Locked;
            mRigidbody = GetComponent<Rigidbody>();
            flashLightController = GetComponentInChildren<FlashLightController>();
            var lights = GetComponentsInChildren<Light>();
            foreach (var mLight in lights)
                if (mLight.enabled)
                    Debug.Log("REMEMBER TO TURN OFF DEV FLASH LIGHT");
        }


        private void Update()
        {
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

#if UNITY_EDITOR

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
#endif

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
            var ray = cameraToControl.ScreenPointToRay(Input.mousePosition);
            SetCrossHairOutline(defaultCrossHairColor, 0.0f); // set black cross-hair
            if (!Physics.Raycast(ray, out hit, interactableLayerMask)) return;
            var isOutOfRange = IsInteractOutOfRange(transform.position, hit.point);
            var hitInteractable = hit.transform.gameObject.TryGetComponent(out Interactable interactableObject);
            if (hitInteractable)
            {
                var canBeInteractedWith = interactableObject.isInteractable;
                if (playerHasControl && canBeInteractedWith) // set white cross-hair
                    SetCrossHairOutline(isInteractableCrossHairColor);
                if (!isOutOfRange && canBeInteractedWith)
                    SetCrossHairOutline(canInteractCrossHairColor); // set green cross-hair
                if (!isOutOfRange && interactableObject.gameObject.TryGetComponent(out KitchenDoorController kDc))
                    if (!hasPickedDrill)
                        SetCrossHairOutline(isNotInteractableCrossHairColor); // set red cross-hair
            }


            if (!Input.GetMouseButtonDown(0) || !playerHasControl || !hitInteractable || isOutOfRange) return;
            interactableObject.Interact(this, lerpAudioSourceSettings);
            if (interactableObject.turnOffFlashLight)
                flashLightController.SetFlashLightTurnedOn(false);
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
    }

    public interface IPlayer
    {
        public PlayerController GetPlayerController();
    }
}