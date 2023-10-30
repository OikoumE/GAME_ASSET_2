using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ReadingTabletController : MonoBehaviour, I_Interactible
{
    public Camera playerCam, lerpCam, fixedCam;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private Color textColor = Color.red;
    [TextAreaAttribute] [SerializeField] private string dialogueText;
    private readonly float textLerpSpeed = 2.5f;
    private readonly float textLerpWait = 1f;
    private readonly float waitAmount = .5f;


    private bool doLerp;
    private bool doTextLerp;
    private Camera fromCam, toCam;
    private int lastPanel;
    private float lerpAlpha, colorLerpAlpha;
    private Color lerpToColor = Color.clear, lerpFromColor = Color.red;

    private PlayerController pC;

    private TextMeshProUGUI tmpText;
    private GameObject toCamObject;
    private Vector3 toCamObjectPosition;
    private Quaternion toCamObjectRotation;
    private Transform toCamObjectTransform;

    public bool InteractModeEnabled { get; private set; }

    private void Start()
    {
        SetText();
    }

    private void Update()
    {
        DoRay();
        LerpTextAlpha();
        if (!doLerp || !pC) return;
        LerpToCam();
    }

    private void OnValidate()
    {
        SetText();
    }


    public void Interact(int panelId, PlayerController _pC)
    {
        _pC.SetPlayerControl(false);
        if (!InteractModeEnabled) StartCoroutine(FadeText());

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
        if (!InteractModeEnabled) return;
        if (!Input.GetMouseButtonDown(0))
            return; // mouse is not clicked or we are hovering activeFloor;
        PlayAudio(audioSettings.clickSettings);
        // button has been clicked and cursor is not over activeFloor;
        StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        StartCoroutine(FadeText());
        yield return new WaitForSeconds(textLerpWait); // cooldown before returning to playerCam.
        pC.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
        yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
        Interact(lastPanel, pC); // trigger lerp back to player
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
            lerpAlpha = 0;
            doLerp = false;
            if (InteractModeEnabled)
                pC.SetPlayerControl(true);
            else pC.SetCursorLockMode(CursorLockMode.None); // toggle cursor on / unlock mouse
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
            if (!InteractModeEnabled) SetTextEnabled(false);
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