using System;
using System.Collections;
using Controllers;
using Interactables;
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
        private ElevatorController elevatorController;
        private Vector3 hitPoint;

        private void Start()
        {
            elevatorController = GetComponentInParent<ElevatorController>();
        }

        protected override void Update()
        {
            base.Update();
            DoRay();

            LerpToCam(audioSettings.lerpSettings);
        }

        private void OnDrawGizmos()
        {
            if (!toCamObject) return;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(toCamObject.transform.position, hitPoint);
        }

        public override void Interact(KitchenDoorController kDC)
        {
            throw new NotImplementedException();
        }

        public override void Interact(FuseboxController fC)
        {
            throw new NotImplementedException();
        }

        private void SetColliderState(bool state)
        {
            foreach (var box in boxColliders) box.enabled = state;
        }

        private void DoRay()
        {
            if (!InteractModeEnabled) return;
            var activeFloor = elevatorController.ActiveFloor;
            var ray = fixedCam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000)) return;
            hitPoint = hit.point;
            var objectHit = hit.transform;

            var isElevatorButtonHitBox = objectHit.gameObject.TryGetComponent(out ElevatorButtonHitBox eButtonHitBox);
            if (!isElevatorButtonHitBox)
            {
                Cursor.SetCursor(redHand, Vector2.zero, CursorMode.Auto);
                playerController.SetCrossHairEnabled(false);
                return;
            }

            var handColor = activeFloor == eButtonHitBox.floor ? redHand : greenHand;
            Cursor.SetCursor(handColor, Vector2.zero, CursorMode.Auto);

            if (!Input.GetMouseButtonDown(0))
                return; // mouse is not clicked or we are hovering activeFloor;

            if (activeFloor == eButtonHitBox.floor)
            {
                PlayAudio(audioSettings.errorSettings);
                return;
            }

            PlayAudio(audioSettings.clickSettings);
            // button has been clicked and cursor is not over activeFloor;
            elevatorController.ActiveFloor = eButtonHitBox.floor; // set active floor
            StartCoroutine(ReturnToPlayer());
        }

        protected override IEnumerator ReturnToPlayer()
        {
            playerController.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
            var waitAmount = elevatorController.runtimeValues.returnToPlayerCooldown;
            yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
            Interact(playerController); // trigger lerp back to player
            yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
            playerController.SetCrossHairEnabled(true);
            StartCoroutine(elevatorController.MoveElevatorToFloor()); // close doors.
        }


        protected override void LerpToCam(AudioSourceSettings audioSourceSettings, bool interruptAudio = false)
        {
            if (!doLerp || !playerController) return;
            if (lerpAlpha >= 1)
                SetColliderState(InteractModeEnabled);
            base.LerpToCam(audioSourceSettings, interruptAudio);
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