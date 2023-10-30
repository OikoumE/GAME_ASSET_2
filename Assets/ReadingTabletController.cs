using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ReadingTabletController : MonoBehaviour, I_Interactible
{
    public Camera playerCam, lerpCam, fixedCam;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private float waitAmount;

    private bool doLerp;
    private Camera fromCam, toCam;

    [SerializeField] private TextField inputText;
    private int lastPanel;
    [Range(0, 1)] private float lerpAlpha;

    private PlayerController pC;

    private TextMeshProUGUI tmpText;
    private GameObject toCamObject;
    private Vector3 toCamObjectPosition;
    private Quaternion toCamObjectRotation;
    private Transform toCamObjectTransform;

    public bool InteractModeEnabled { get; private set; }


    private void Start()
    {
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText == null) throw new Exception("TMP not found");
    }

    private void Update()
    {
        DoRay();
        if (!doLerp || !pC) return;
        LerpToCam();
    }


    public void Interact(int panelId, PlayerController _pC)
    {
        _pC.SetPlayerControl(false);
        Debug.Log("isTablet clicked: ");

        #region Setting From/To cam

        pC = _pC;
        fromCam = _pC.playerCam;
        toCamObject = fixedCam.gameObject;
        toCam = fixedCam;
        if (InteractModeEnabled)
        {
            fromCam = toCam;
            toCamObject = playerCam.gameObject;
            toCam = playerCam;
        }

        #endregion

        toCamObjectTransform = toCamObject.transform;
        toCamObjectPosition = toCamObjectTransform.position;
        toCamObjectRotation = toCamObjectTransform.rotation;

        doLerp = true; // trigger lerp
    }


    private void PlayAudio(AudioSourceSettings settings, bool interrupt = true)
    {
        var audioSource = settings.Source;
        if (interrupt) audioSource.Stop();
        audioSource.pitch = settings.pitch;
        audioSource.volume = settings.volume;
        audioSource.clip = settings.audioClip;
        audioSource.Play();
    }

    private void DoRay()
    {
        if (!InteractModeEnabled) return;
        if (!Input.GetMouseButtonDown(0))
            return; // mouse is not clicked or we are hovering activeFloor;
        PlayAudio(audioSettings.clickSettings);
        // button has been clicked and cursor is not over activeFloor;
        StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        pC.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
        yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
        Interact(lastPanel, pC); // trigger lerp back to player
    }


    private void LerpToCam()
    {
        if (!doLerp) return;
        if (lerpAlpha < 1)
            lerpAlpha += Time.deltaTime * lerpSpeed;
        if (!(audioSettings.lerpSettings.Source.clip == audioSettings.lerpSettings.audioClip))
            PlayAudio(audioSettings.lerpSettings);
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

        // when lerp is 1 or more, set "interactCam" active, deactivate lerpcam
        if (lerpAlpha >= 1)
        {
            lerpCam.enabled = false;
            toCam.enabled = true;
            lerpAlpha = 0;
            doLerp = false;
            if (InteractModeEnabled) pC.SetPlayerControl(true);
            else pC.SetCursorLockMode(CursorLockMode.None); // toggle cursor on / unlock mouse
            InteractModeEnabled = !InteractModeEnabled;
        }
    }

    [Serializable]
    private class AudioSettings
    {
        [SerializeField] public AudioSourceSettings clickSettings;
        [SerializeField] public AudioSourceSettings lerpSettings;
        [SerializeField] public AudioClip lerpCamAudioClip;
        [SerializeField] public AudioClip buttonClickAudioClip;
    }
}