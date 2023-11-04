using System;
using System.Collections;
using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerController pC);
    public void Interact(KitchenDoorController kDC);
}

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public Camera lerpCam, fixedCam;
    public float lerpSpeed = 1f;
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

    public virtual void Interact(PlayerController pC)
    {
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

    protected virtual IEnumerator ReturnToPlayer()
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

    protected virtual void LerpToCam(AudioSourceSettings audioSourceSettings, bool interruptAudio = false)
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