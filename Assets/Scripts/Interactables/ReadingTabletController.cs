using System;
using System.Collections;
using Controllers;
using Elevator;
using TMPro;
using UnityEngine;

namespace Interactables
{
    public class ReadingTabletController : Interactable
    {
        [SerializeField] private AudioSettings audioSettings;
        [SerializeField] private Color textColor = Color.red;
        [TextArea] [SerializeField] private string dialogueText;
        private readonly float textLerpSpeed = 2.5f;
        private readonly float textLerpWait = 1f;
        private readonly float waitAmount = .5f;
        private float colorLerpAlpha;

        private bool doTextLerp;
        private bool hasLerpedText;

        private bool isReturning;
        private Color lerpToColor = Color.clear, lerpFromColor = Color.red;

        private TextMeshProUGUI tmpText;

        private void Start()
        {
            if (fromCam == null) fromCam = Camera.main;
            SetText();
        }

        protected override void Update()
        {
            base.Update();
            DoRay();
            LerpTextAlpha();
            LerpToCam(audioSettings.lerpSettings, true);
        }

        private void OnValidate()
        {
            SetText();
        }


        public override void Interact(PlayerController pC)
        {
            playerController = pC;
            if (!InteractModeEnabled) StartCoroutine(FadeText());
            pC.SetCrossHairEnabled(InteractModeEnabled);
            base.Interact(pC);
        }

        public override void Interact(KitchenDoorController kDC)
        {
            throw new NotImplementedException();
        }

        public override void Interact(FuseboxController fC)
        {
            throw new NotImplementedException();
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

        private void DoRay()
        {
            if (!InteractModeEnabled || isReturning) return;
            if (!Input.GetMouseButtonDown(0)) return;
            isReturning = true;
            PlayAudio(audioSettings.clickSettings);
            // button has been clicked and cursor is not over activeFloor;
            StartCoroutine(ReturnToPlayer());
        }

        protected override IEnumerator ReturnToPlayer()
        {
            StartCoroutine(FadeText());
            yield return new WaitForSeconds(textLerpWait); // cooldown before returning to playerCam.
            playerController.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
            yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
            Interact(playerController); // trigger lerp back to player
        }

        protected override void LerpToCam(AudioSourceSettings audioSourceSettings, bool interruptAudio = false)
        {
            if (!doLerp || !playerController) return;
            if (lerpAlpha >= 1)
                isReturning = false;
            base.LerpToCam(audioSourceSettings, interruptAudio);
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
}