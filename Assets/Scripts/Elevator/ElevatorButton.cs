using System;
using System.Collections;
using Controllers;
using Interactables;
using Unity.VisualScripting;
using UnityEngine;

namespace Elevator
{
    public enum Floor
    {
        Engine,
        Restaurant,
        Kitchen
    }

    public class ElevatorButton : Interactable
    {
        [SerializeField] private Texture2D greenHand, redHand;
        [SerializeField] private AudioSettings audioSettings;
        [SerializeField] private BoxCollider[] boxColliders;
        [SerializeField] private GameObject unInteractButton;
        [DoNotSerialize] public bool isPlayerInsideElevator;
        private ElevatorController elevatorController;
        private Vector3 hitPoint;

        private bool isCursorOverUi;

        private void Start()
        {
            elevatorController = GetComponentInParent<ElevatorController>();
        }

        protected override void Update()
        {
            base.Update();
            DoRay();

            LerpToCam();
        }

        private void OnDrawGizmos()
        {
            if (!toCamObject) return;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(toCamObject.transform.position, hitPoint);
        }


        private void SetColliderState(bool state)
        {
            foreach (var box in boxColliders) box.enabled = state;
        }

        public override void UnInteract(PlayerController pC)
        {
            if (!InteractModeEnabled) return;
            isCursorOverUi = false;
            pC.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
            Interact(playerController, audioSettings.lerpSettings);
            pC.SetCrossHairEnabled(true);
        }

        public override void Interact(
            PlayerController pC,
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio = true
        )
        {
            if (unInteractButton.activeInHierarchy)
                unInteractButton.SetActive(false);
            if (!isPlayerInsideElevator) return;


            base.Interact(pC, audioSourceSettings, interruptAudio);
        }

        public void SetCursorUi(bool setAsGreen)
        {
            var colorToSet = setAsGreen ? greenHand : redHand;
            isCursorOverUi = setAsGreen;
            Cursor.SetCursor(colorToSet, Vector2.zero, CursorMode.Auto);
        }

        private void DoRay()
        {
            if (!InteractModeEnabled) return;

            var activeFloor = elevatorController.ActiveFloor;
            var ray = fixedCam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000)) return;
            hitPoint = hit.point;
            var objectHit = hit.transform;
            if (isCursorOverUi) return;
            var isElevatorButtonHitBox = objectHit.gameObject.TryGetComponent(out ElevatorButtonHitBox eButtonHitBox);
            if (!isElevatorButtonHitBox)
            {
                Cursor.SetCursor(redHand, Vector2.zero, CursorMode.Auto);
                return;
            }


            var handColor = activeFloor == eButtonHitBox.floor ? redHand : greenHand;
            var xHotSpot = handColor.width / 2f;
            var a = new Vector2(xHotSpot, 0);
            Cursor.SetCursor(handColor, Vector2.zero, CursorMode.Auto);

            if (!Input.GetMouseButtonDown(0))
                return; // mouse is not clicked or we are hovering activeFloor;

            if (activeFloor == eButtonHitBox.floor)
            {
                PlayAudio(audioSettings.errorSettings);
                return;
            }

            // button has been clicked and cursor is not over activeFloor;
            PlayAudio(audioSettings.clickSettings);
            elevatorController.ActiveFloor = eButtonHitBox.floor; // set active floor
            StartCoroutine(ReturnToPlayer(audioSettings.lerpSettings, true));
        }

        protected override IEnumerator ReturnToPlayer(
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio)
        {
            playerController.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
            var waitAmount = elevatorController.runtimeValues.returnToPlayerCooldown;
            yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
            Interact(playerController, audioSourceSettings, interruptAudio); // trigger lerp back to player
            StartCoroutine(elevatorController.MoveElevatorToFloor()); // close doors.
            yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
            playerController.SetCrossHairEnabled(true);
        }


        protected override void LerpToCam()
        {
            if (!doLerp || !playerController) return;
            if (lerpAlpha >= 1)
            {
                SetColliderState(InteractModeEnabled);
                if (!unInteractButton.activeInHierarchy && !InteractModeEnabled)
                    unInteractButton.SetActive(true);
            }

            playerController.SetCrossHairEnabled(InteractModeEnabled);
            base.LerpToCam();
        }

        [Serializable]
        private class AudioSettings
        {
            [SerializeField] public AudioSourceSettings errorSettings;
            [SerializeField] public AudioSourceSettings clickSettings;
            [SerializeField] public AudioSourceSettings lerpSettings;
            [SerializeField] public AudioClip lerpCamAudioClip;
            [SerializeField] public AudioClip buttonClickAudioClip;
            [SerializeField] public AudioClip buttonErrorAudioClip;
        }
    }
}