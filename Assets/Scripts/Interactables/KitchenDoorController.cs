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
        private float doorTranslateLerpAlpha;
        private float drillLerpAlpha;


        private bool hasReturnedToPlayer;

        private bool hasStartedDrill;
        [HideInInspector] public RaycastHit hit;
        private AudioSource onWireConnectAudioSource;
        private Vector3 startPos;


        private void Start()
        {
            drillPrefab.SetActive(false);
            var audioSources = wireGameAudio.GetComponents<AudioSource>();
            onWireConnectAudioSource = audioSources[1];
            boxCollider = GetComponent<BoxCollider>();
            if (!doorMesh || !kitchenDoorRayCollider) throw new Exception("check component references!");
            doorMeshRb = doorMesh.GetComponent<Rigidbody>();
            doorMeshRb.isKinematic = true;
            doorMeshRb.useGravity = false;

            if (wireTargetColliders.Count < 6) throw new Exception("Set wireTargetColliders");

            startPos = doorRotatePoint.position;
            SetCamController();
            if (animateDoor) throw new Exception("Remember to disable animateDoor");
        }


        protected override void Update()
        {
            base.Update();
            AnimateDrill();
            LerpToCam(lerpCamSetting);
            if (numberOfConnectedWires == 6 && !hasReturnedToPlayer)
            {
                // we have all wires connected!
                hasReturnedToPlayer = true;
                StartCoroutine(ReturnToPlayer());
            }
            // todo incorporate drill, drill mechanics, drill animations, sounds
            // todo prevent for interacting with panel without drill

            if (cameraHasControl)
                cameraController.RunInputHandler();
            DoRay();
            DoDoorAnimation();
        }


        private void OnValidate()
        {
            SetCamController();
        }


        private IEnumerator FreezeCamForRetractingDrill(float duration)
        {
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
            if (numberOfConnectedWires == 6) return;
            if (playerController)
                playerController.SetCrossHairOutline(cameraController.IsInteractableCrossHairColor, 0);
            var ray = cameraController.cameraToControl.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit, cameraController.interactableLayerMask)) return;
            var hitInteractable = hit.transform.gameObject.TryGetComponent(out Interactable interactableObject);
            if (hitInteractable)
                if (playerController && interactableObject.isInteractable)
                    playerController.SetCrossHairOutline(cameraController.CanInteractCrossHairColor);
            if (!Input.GetMouseButtonDown(0) || !cameraHasControl || !hitInteractable) return;
            interactableObject.Interact(this);

            if (!drillPrefab.activeSelf) return;
            drillTargetPosition.position = hit.transform.position;
            animateDrill = true;
            StartCoroutine(FreezeCamForRetractingDrill(drillAnimDuration));
        }

        public void RetractDrill()
        {
            if (!animateDrillRetract) return;


            if (drillLerpAlpha < 1) drillLerpAlpha += Time.deltaTime * drillLerpSpeed;
            var drillEndPos = drillInHandPosition;

            if (animateDoor) drillEndPos = drillOutOfCamPosition;


            drillPrefab.transform.localPosition =
                Vector3.Lerp(drillTargetPosition.localPosition, drillEndPos, drillLerpAlpha);
            if (drillLerpAlpha >= 1)
            {
                drillLerpAlpha = 0;
                animateDrillRetract = false;
            }
        }

        private void AnimateDrillIntro()
        {
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

        protected override IEnumerator ReturnToPlayer()
        {
            playerController.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
            yield return new WaitForSeconds(waitAfterComplete); // cooldown before returning to playerCam.
            Interact(playerController); // trigger lerp back to player
        }

        private void DoDoorAnimation()
        {
            if (!animateDoor) return;
            if (doorTranslateLerpAlpha < 1)
                doorTranslateLerpAlpha += Time.deltaTime * doorTranslateSpeed;
            doorRotatePoint.transform.position = Vector3.Lerp(startPos, endPos.position, doorTranslateLerpAlpha);
            if (doorTranslateLerpAlpha <= 1) return;
            doorRotatePoint.transform.Rotate(Vector3.up, -doorRotateSpeed, Space.Self);
            if (doorRotatePoint.transform.rotation.y >= 0.69f) return; // magic number, trust me bro!
            drillPrefab.gameObject.SetActive(false);
            SetWireTargetsEnabled(true);
            doorMeshRb.isKinematic = false;
            doorIsOpen = true;
            doorMeshRb.useGravity = true;
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
            // lerp lerpCam to InteractCam
            lerpCam.transform.position = Vector3.Lerp(
                fromCam.transform.position,
                toCamObjectPosition,
                lerpAlpha
            );
            lerpCam.transform.rotation = Quaternion.Lerp(
                fromCam.transform.rotation,
                toCamObjectRotation,
                lerpAlpha);

            if (!lerpCam.enabled)
            {
                lerpCam.enabled = true;
                fromCam.enabled = false;
            }

            if (lerpAlpha <= 1) return;
            lerpCam.enabled = false;
            toCam.enabled = true;
            lerpAlpha = 0;
            doLerp = false;
            if (InteractModeEnabled)
            {
                playerController.SetPlayerControl(true);
            }

            
            kitchenDoorRayCollider.enabled = !InteractModeEnabled;
            boxCollider.enabled = false;
            cameraHasControl = !InteractModeEnabled;
            StartCoroutine(FreezeCamForDuration(initialWaitForControl));
            drillPrefab.SetActive(!InteractModeEnabled);
            InteractModeEnabled = !InteractModeEnabled;
        }


        private IEnumerator FreezeCamForDuration(float duration)
        {
            cameraHasControl = false;
            yield return new WaitForSeconds(duration);
            cameraHasControl = true;
        }

        private void SetWireTargetsEnabled(bool enable)
        {
            foreach (var wireTargetCollider in wireTargetColliders) wireTargetCollider.enabled = enable;
        }
    }
}