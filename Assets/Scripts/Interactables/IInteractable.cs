using System;
using System.Collections;
using Controllers;
using Elevator;
using UnityEngine;

namespace Interactables
{
    public interface IInteractable
    {
        public void Interact(PlayerController pC, AudioSourceSettings audioSourceSettings, bool interruptAudio = true);
        public void Interact(KitchenDoorController kDC);
        public void Interact(FuseboxController fC);

        public void UnInteract(PlayerController playerController);
    }

    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [Header("Interactable Settings")] public Camera lerpCam;
        public Camera fixedCam;
        public float lerpSpeed = 1f;
        public bool isInteractable = true;

        [HideInInspector] public float lerpAlpha;
        protected bool doLerp;
        protected PlayerController playerController;
        protected Camera toCam, fromCam;
        protected GameObject toCamObject;
        protected Vector3 toCamObjectPosition;
        protected Quaternion toCamObjectRotation;
        protected Transform toCamObjectTransform;
        protected bool InteractModeEnabled { get; set; }

        protected virtual void Update()
        {
            CalculateLerpAlpha();
        }

        public virtual void Interact(
            PlayerController pC,
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio = true
        )
        {
            if (audioSourceSettings.Source.clip != audioSourceSettings.audioClip || interruptAudio)
                PlayAudio(audioSourceSettings);


            pC.SetPlayerControl(false);
            playerController = pC;
            fromCam = pC.PlayerCam;
            toCam = fixedCam;
            toCamObject = toCam.gameObject;
            if (InteractModeEnabled)
            {
                fromCam = toCam;
                toCam = pC.PlayerCam;
                toCamObject = pC.PlayerCam.gameObject;
            }


            toCamObjectTransform = toCamObject.transform;
            toCamObjectPosition = toCamObjectTransform.position;
            toCamObjectRotation = toCamObjectTransform.rotation;


            doLerp = true; // trigger lerp
        }

        public virtual void Interact(KitchenDoorController kDC)
        {
            throw new NotImplementedException();
        }

        public virtual void Interact(FuseboxController fC)
        {
            throw new NotImplementedException();
        }

        public virtual void UnInteract(PlayerController playerController)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerator ReturnToPlayer(
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio)
        {
            throw new NotImplementedException();
        }

        protected static void PlayAudio(AudioSourceSettings settings, bool interrupt = true)
        {
            var audioSource = settings.Source;
            if (interrupt) audioSource.Stop();
            audioSource.pitch = settings.pitch;
            audioSource.volume = settings.volume;
            audioSource.clip = settings.audioClip;
            audioSource.Play();
        }

        private void CalculateLerpAlpha()
        {
            if (!doLerp) return;
            if (lerpAlpha < 1)
                lerpAlpha += Time.deltaTime * lerpSpeed;
        }

        protected virtual void LerpToCam()
        {
            if (!doLerp) return;
            // if (audioSourceSettings.Source.clip != audioSourceSettings.audioClip || interruptAudio)
            //     PlayAudio(audioSourceSettings);
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
                else playerController.SetCursorLockMode(CursorLockMode.None); // toggle cursor on / unlock mouse
                InteractModeEnabled = !InteractModeEnabled;
            }
        }
    }
}