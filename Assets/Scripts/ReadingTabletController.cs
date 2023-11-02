using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ReadingTabletController : Interactable
{
    public Camera playerCam, lerpCam, fixedCam;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private Color textColor = Color.red;
    [SerializeField] private Material readingTabledEmit;
    [TextAreaAttribute] [SerializeField] private string dialogueText;

    [SerializeField] private Color screenOnColor;
    private readonly float textLerpSpeed = 2.5f;
    private readonly float textLerpWait = 1f;
    private readonly float waitAmount = .5f;


    private bool doLerp;
    private bool doTextLerp;
    private Camera fromCam, toCam;
    private bool hasLerpedText;

    private bool isReturning;
    private float lerpAlpha, colorLerpAlpha;
    private Color lerpToColor = Color.clear, lerpFromColor = Color.red;

    private PlayerController playerController;

    private TextMeshProUGUI tmpText;
    private GameObject toCamObject;
    private Vector3 toCamObjectPosition;
    private Quaternion toCamObjectRotation;
    private Transform toCamObjectTransform;

    public bool InteractModeEnabled { get; private set; }

    private void Start()
    {
        SetText();
        if (playerCam == null) playerCam = Camera.main;
    }

    private void Update()
    {
        DoRay();
        LerpTextAlpha();
        if (!doLerp || !playerController) return;
        LerpToCam();
    }

    private void OnValidate()
    {
        SetText();
    }


    public override void Interact(PlayerController pC, int panelId)
    {
        throw new NotImplementedException();
    }

    public override void Interact(PlayerController pC)
    {
        pC.SetPlayerControl(false);
        if (!InteractModeEnabled) StartCoroutine(FadeText());

        #region Setting From/To cam

        playerController = pC;
        fromCam = pC.PlayerCam;
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

    private void SetText()
    {
        if (tmpText == null) tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText == null) throw new Exception("TMP not found");
        tmpText.text = dialogueText;
        tmpText.color = Color.clear;
        SetTextEnabled(false);
    }

    private void SetTextEnabled(bool enable)
    {
        tmpText.enabled = enable;
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
        if (!InteractModeEnabled || isReturning) return;
        if (!Input.GetMouseButtonDown(0)) return;
        isReturning = true;
        PlayAudio(audioSettings.clickSettings);
        // button has been clicked and cursor is not over activeFloor;
        StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        StartCoroutine(FadeText());
        yield return new WaitForSeconds(textLerpWait); // cooldown before returning to playerCam.
        playerController.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
        yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
        Interact(playerController); // trigger lerp back to player
    }


    private void LerpToCam()
    {
        if (!doLerp) return;
        if (lerpAlpha < 1)
            lerpAlpha += Time.deltaTime * lerpSpeed;
        // if (!(audioSettings.lerpSettings.Source.clip == audioSettings.lerpSettings.audioClip))
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
            isReturning = false;
            lerpAlpha = 0;
            doLerp = false;
            if (InteractModeEnabled)
                playerController.SetPlayerControl(true);
            else playerController.SetCursorLockMode(CursorLockMode.None); // toggle cursor on / unlock mouse
            InteractModeEnabled = !InteractModeEnabled;
        }
    }

    private void LerpTextAlpha()
    {
        if (!doTextLerp) return;

        if (colorLerpAlpha < 1)
            colorLerpAlpha += Time.deltaTime * textLerpSpeed;

        if (colorLerpAlpha >= 1)
        {
            doTextLerp = false;
            colorLerpAlpha = 0;
            if (hasLerpedText) SetTextEnabled(false);
            hasLerpedText = !hasLerpedText;
            return;
        }

        tmpText.color = Color.Lerp(lerpFromColor, lerpToColor, colorLerpAlpha);
    }


    private IEnumerator FadeText()
    {
        if (InteractModeEnabled)
        {
            lerpToColor = Color.clear;
            lerpFromColor = textColor;
        }
        else
        {
            lerpToColor = textColor;
            lerpFromColor = Color.clear;
        }

        SetTextEnabled(true);
        yield return new WaitForSeconds(textLerpWait);
        doTextLerp = true;
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