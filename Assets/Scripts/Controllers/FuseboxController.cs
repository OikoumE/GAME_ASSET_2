using System;
using System.Collections;
using Elevator;
using Interactables;
using StateMachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers
{
    public class FuseboxController : Interactable
    {
        [SerializeField] private GameObject fuseboxLid, fuseToInsert, fuseToRemove;
        [SerializeField] private float maxRotation = 90, startRotation = 20;
        [DoNotSerialize] public bool animateDoor;
        [SerializeField] private AudioSettings lerpAudio;
        private BoxCollider boxCollider;
        private CameraController cameraController;
        private bool cameraHasControl;


        private bool isFuseboxOpen, animateFuse;
        private float rot, fuseLerpAlpha;

        public GameObject FuseToInsert => fuseToInsert;
        public GameObject FuseToRemove => fuseToRemove;

        public PlayerController GetPlayerController => playerController;


        private void Start()
        {
            rot = startRotation;
            SetCamController();
            boxCollider = GetComponent<BoxCollider>();
        }

        protected override void Update()
        {
            var currentStateIsFuseBox = GameStateMachine.Instance.IsCurrentState(GameStateName.FuseBoxState);
            if (!currentStateIsFuseBox) return;
            base.Update();
            if (cameraHasControl) // enables easy disabling of camera control
                cameraController.RunInputHandler();
            DoRay();
            AnimateDoor();
            LerpToCam();
        }


        private void OnValidate()
        {
            SetCamController();
        }


        private void SetCamController()
        {
            cameraController = fixedCam.gameObject.GetComponent<CameraController>();
        }

        public override void Interact(
            PlayerController pC,
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio = true
        )
        {
            boxCollider.enabled = false;
            playerController = pC;
            if (InteractModeEnabled)
                StartCoroutine(SetNextStateDelayed());

            base.Interact(pC, audioSourceSettings, interruptAudio);
        }

        private IEnumerator SetNextStateDelayed()
        {
            yield return new WaitForSeconds(1.5f);
            GameStateMachine.Instance.SetState(GameStateMachine.Instance.wireGameState);
        }

        private void DoRay()
        {
            //handles all rayCasting
            if (!cameraHasControl) return;
            if (playerController) // sets crossHair to default
                playerController.SetCrossHairOutline(cameraController.IsInteractableCrossHairColor, 0);
            // create ray
            var ray = cameraController.cameraToControl.ScreenPointToRay(Input.mousePosition);
            //cast ray
            if (!Physics.Raycast(ray, out var hit, cameraController.interactableLayerMask)) return;
            // check if we hit an interactable object
            var hitInteractable = hit.transform.gameObject.TryGetComponent(out Interactable interactableObject);
            if (hitInteractable) // set crossHair if its an interactable, and can be interacted with
                if (playerController && interactableObject.isInteractable)
                    playerController.SetCrossHairOutline(cameraController.CanInteractCrossHairColor);
            if (!Input.GetMouseButtonDown(0) || !cameraHasControl || !hitInteractable) return;
            // all conditions for interaction has been met
            interactableObject.Interact(this);
        }

        protected override void LerpToCam()
        {
            if (!doLerp) return;

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

            if (lerpAlpha >= 1)
            {
                lerpCam.enabled = false;
                toCam.enabled = true;
                lerpAlpha = 0;

                doLerp = false;
                if (InteractModeEnabled) playerController.SetPlayerControl(true);
                else
                    GameStateMachine.Instance.SetCursorLockMode(CursorLockMode
                        .Locked); // toggle cursor on / unlock mouse
                InteractModeEnabled = !InteractModeEnabled;
                cameraHasControl = InteractModeEnabled;
            }
        }

        private void AnimateDoor()
        {
            if (animateDoor)
            {
                OpenFusebox();
                CloseFusebox();
            }

            fuseboxLid.transform.localRotation = Quaternion.Euler(0, 0, rot);
        }

        private void OpenFusebox()
        {
            // Open the fusebox lid 
            if (isFuseboxOpen) return;
            if (rot >= -maxRotation)
            {
                rot -= 1;
            }
            else if (rot <= -maxRotation)
            {
                isFuseboxOpen = true;
                animateDoor = false;
            }
        }


        private void CloseFusebox()
        {
            if (!isFuseboxOpen) return;
            if (rot <= 0)
            {
                rot += 1;
            }
            else if (rot >= 0)
            {
                isFuseboxOpen = false;
                animateDoor = false;
            }
        }

        [Serializable]
        private class AudioSettings
        {
            [SerializeField] public AudioSourceSettings lerpSettings;
        }
    }
}