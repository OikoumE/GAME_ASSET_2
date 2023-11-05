using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables
{
    public class KitchenDoorController : Interactable
    {
        [Header("Door Settings")] public bool animateDoor;

        [HideInInspector] public bool doorIsOpen;
        [SerializeField] private Transform endPos, doorRotatePoint;
        [SerializeField] private float doorTranslateSpeed = 1, doorRotateSpeed = 1;
        [SerializeField] private GameObject doorMesh;

        [FormerlySerializedAs("mMeshCollider")] [SerializeField]
        private MeshCollider kitchenDoorRayCollider;

        [Header("WireGame Settings")] [SerializeField] [HideInInspector]
        public int numberOfConnectedWires;

        [SerializeField] [Range(0, 10)] private float waitAfterComplete = 1f;
        [SerializeField] private List<BoxCollider> wireTargetColliders;

        [Header("Audio/Cam Settings")] [SerializeField]
        private AudioSourceSettings lerpCamSetting;

        [SerializeField] private GameObject wireGameAudio;
        [SerializeField] private float initialWaitForControl;

        [Header("Drill Settings")] [SerializeField]
        private GameObject drillPrefab;

        [SerializeField] private float drillLerpSpeed;
        [SerializeField] private float drillAnimDuration = 3f;
        [SerializeField] private Transform drillTargetPosition;
        private readonly Vector3 drillInHandPosition = new(0.217634f, -0.2045913f, 0.7481999f);
        private readonly Vector3 drillOutOfCamPosition = new(0.279000014f, -0.177000001f, 0.256999999f);


        private bool animateDrill;
        private bool animateDrillRetract;
        private BoxCollider boxCollider;
        private CameraController cameraController;
        private bool cameraHasControl;
        private Rigidbody doorMeshRb;
        private Vector3 doorRotateStartPos;
        private float doorTranslateLerpAlpha;
        private float drillLerpAlpha;


        private bool hasReturnedToPlayer;

        private bool hasStartedDrill;
        [HideInInspector] public RaycastHit hit;
        private AudioSource onWireConnectAudioSource;


        private void Start()
        {
            drillPrefab.SetActive(false);
            var audioSources = wireGameAudio.GetComponents<AudioSource>();
            onWireConnectAudioSource = audioSources[1];
            boxCollider = GetComponent<BoxCollider>();
            doorMeshRb = doorMesh.GetComponent<Rigidbody>();
            doorMeshRb.isKinematic = true;
            doorMeshRb.useGravity = false;
            doorRotateStartPos = doorRotatePoint.position;
            SetCamController();
            if (!doorMesh || !kitchenDoorRayCollider) throw new Exception("check component references!");
            if (wireTargetColliders.Count < 6) throw new Exception("Set wireTargetColliders");
            if (animateDoor) throw new Exception("Remember to disable animateDoor");
        }

        protected override void Update()
        {
            base.Update(); // runs lerp
            AnimateDrill(); // handles all drill animation
            LerpToCam(lerpCamSetting); // handles lerp-ing to/from camera
            if (numberOfConnectedWires == 6 && !hasReturnedToPlayer)
            {
                // we have all wires connected!
                hasReturnedToPlayer = true; // make sure we only trigger once
                StartCoroutine(ReturnToPlayer());
            }
            // todo prevent for interacting with panel without drill

            if (cameraHasControl) // enables easy disabling of camera control
                cameraController.RunInputHandler();
            DoRay(); // handles raycast for interactions
            DoDoorAnimation(); // handles animating the removal of door panel
        }


        private void OnValidate()
        {
            SetCamController();
        }


        private IEnumerator FreezeCamForRetractingDrill(float duration)
        {
            // disables camera, waits for a duration
            // enables drillRetract animation and waits for it to finnish
            // returns control to camera and reset lerpAlpha
            cameraHasControl = false;
            yield return new WaitForSeconds(duration);
            animateDrillRetract = true;
            yield return new WaitForSeconds(1);
            cameraHasControl = true;
            drillLerpAlpha = 0;
        }

        private void SetCamController()
        {
            cameraController = fixedCam.gameObject.GetComponent<CameraController>();
        }

        private void DoRay()
        {
            //handles all rayCasting
            if (numberOfConnectedWires == 6)
            {
                // makes sure we dont accidentally leave camera under player control
                cameraHasControl = false;
                return; // if game is done, we are done!
            }

            if (playerController) // sets crossHair to default
                playerController.SetCrossHairOutline(cameraController.IsInteractableCrossHairColor, 0);
            // create ray
            var ray = cameraController.cameraToControl.ScreenPointToRay(Input.mousePosition);
            //cast ray
            if (!Physics.Raycast(ray, out hit, cameraController.interactableLayerMask)) return;
            // check if we hit an interactable object
            var hitInteractable = hit.transform.gameObject.TryGetComponent(out Interactable interactableObject);
            if (hitInteractable) // set crossHair if its an interactable, and can be interacted with
                if (playerController && interactableObject.isInteractable)
                    playerController.SetCrossHairOutline(cameraController.CanInteractCrossHairColor);
            if (!Input.GetMouseButtonDown(0) || !cameraHasControl || !hitInteractable) return;
            // all conditions for interaction has been met
            interactableObject.Interact(this);

            // if drill is active, set drill target position, enable drill animation and freeze camera
            if (!drillPrefab.activeSelf) return;
            drillTargetPosition.position = hit.transform.position;
            animateDrill = true;
            StartCoroutine(FreezeCamForRetractingDrill(drillAnimDuration));
        }

        protected override IEnumerator ReturnToPlayer()
        {
            // toggle cursor off/lock mouse
            playerController.SetCursorLockMode(CursorLockMode.Locked);
            // cooldown before returning to playerCam.
            yield return new WaitForSeconds(waitAfterComplete);
            // trigger lerp back to player
            Interact(playerController);
        }

        private void DoDoorAnimation()
        {
            if (!animateDoor) return;
            if (doorTranslateLerpAlpha < 1)
                doorTranslateLerpAlpha += Time.deltaTime * doorTranslateSpeed;
            doorRotatePoint.transform.position =
                Vector3.Lerp(doorRotateStartPos, endPos.position, doorTranslateLerpAlpha);
            if (doorTranslateLerpAlpha <= 1) return;
            // we are done lerp-ing, start rotating
            doorRotatePoint.transform.Rotate(Vector3.up, -doorRotateSpeed, Space.Self);
            if (doorRotatePoint.transform.rotation.y >= 0.69f) return; // magic number, trust me bro!
            // we have reached target rotation
            // deactivate drillPrefab - we are done with it
            drillPrefab.gameObject.SetActive(false);
            // enable hitBoxes for interaction with wires
            SetWireTargetsEnabled(true);
            // make door fall to floor via rigidbody settings
            doorMeshRb.isKinematic = false;
            doorIsOpen = true;
            doorMeshRb.useGravity = true;
            // disable doorAnimation
            animateDoor = false;
        }

        public void PlayWireConnectedAudio()
        {
            onWireConnectAudioSource.Play();
        }

        protected override void LerpToCam(AudioSourceSettings audioSourceSettings, bool interruptAudio = false)
        {
            if (!doLerp) return;
            if (audioSourceSettings.Source.clip != audioSourceSettings.audioClip || interruptAudio)
                PlayAudio(audioSourceSettings);
            // lerp lerpCam's position to InteractCam's position
            lerpCam.transform.position = Vector3.Lerp(
                fromCam.transform.position,
                toCamObjectPosition,
                lerpAlpha
            );
            // lerp lerpCam's rotation to InteractCam's rotation
            lerpCam.transform.rotation = Quaternion.Lerp(
                fromCam.transform.rotation,
                toCamObjectRotation,
                lerpAlpha);

            if (!lerpCam.enabled)
            {
                //toggle between lerpCam and toCam while we lerp
                lerpCam.enabled = true;
                fromCam.enabled = false;
            }

            if (lerpAlpha <= 1) return;
            // we are done lerp-ing
            lerpCam.enabled = false; //toggle between lerpCam and toCam
            toCam.enabled = true;
            lerpAlpha = 0; //reset lerpAlpha
            doLerp = false; // disable lerp-ing
            cameraHasControl = false; // added wait timer (FreezeCamForDuration) that
            // turn off/on cameraControls
            if (InteractModeEnabled)
                // if we have done lerp, interacted, and are about to lerp back
                playerController.SetPlayerControl(true);
            else // we are on out way to interact
                StartCoroutine(FreezeCamForDuration(initialWaitForControl));

            kitchenDoorRayCollider.enabled = !InteractModeEnabled; // toggle rayCollider
            boxCollider.enabled = false; // disable collider for interacting with door
            drillPrefab.SetActive(!InteractModeEnabled); // toggle drillPrefab
            InteractModeEnabled = !InteractModeEnabled; // toggle InteractModeEnabled
        }


        private IEnumerator FreezeCamForDuration(float duration)
        {
            cameraHasControl = false; // disable cam
            yield return new WaitForSeconds(duration); //wait
            cameraHasControl = true; // enable cam
        }

        private void SetWireTargetsEnabled(bool enable)
        {
            // loops over all wireTargetColliders and sets enabled state
            foreach (var wireTargetCollider in wireTargetColliders) wireTargetCollider.enabled = enable;
        }

        #region DRILL // all the drill animations

        private void RetractDrill()
        {
            if (!animateDrillRetract) return;
            if (drillLerpAlpha < 1) drillLerpAlpha += Time.deltaTime * drillLerpSpeed;
            var drillEndPos = drillInHandPosition; // declare end position
            if (animateDoor)
                // if all screws are removed, set drill position to outside camframe
                drillEndPos = drillOutOfCamPosition;
            // do the lerping
            drillPrefab.transform.localPosition =
                Vector3.Lerp(drillTargetPosition.localPosition, drillEndPos, drillLerpAlpha);
            if (drillLerpAlpha <= 1) return;
            // we are done lerping
            drillLerpAlpha = 0; // reset alpha
            animateDrillRetract = false; // disable retractDrill
        }

        private void AnimateDrillIntro()
        {
            // will only trigger if we have not gotten control yet (just done lerp-ing)
            // and will only run once due to hasStartedDrill
            if (!cameraHasControl || hasStartedDrill) return;
            if (drillLerpAlpha < 1) drillLerpAlpha += Time.deltaTime * drillLerpSpeed;
            drillPrefab.transform.localPosition =
                Vector3.Lerp(drillOutOfCamPosition, drillInHandPosition, drillLerpAlpha);
            if (drillLerpAlpha <= 1) return;
            hasStartedDrill = true;
            drillLerpAlpha = 0;
        }

        private void AnimateDrill()
        {
            // runs both logic for animating drill on start, during interact, and after
            AnimateDrillIntro();
            RetractDrill();
            if (!animateDrill) return;
            if (drillLerpAlpha < 1) drillLerpAlpha += Time.deltaTime * drillLerpSpeed;

            drillPrefab.transform.localPosition =
                Vector3.Lerp(drillInHandPosition, drillTargetPosition.localPosition, drillLerpAlpha);

            if (drillLerpAlpha >= 1)
            {
                drillLerpAlpha = 0;
                animateDrill = false;
            }
        }

        #endregion
    }
}