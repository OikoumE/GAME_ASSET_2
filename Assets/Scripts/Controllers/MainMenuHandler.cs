using System.Collections;
using StateMachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField] private Camera mainMenuCamera;
        [SerializeField] private float fadeToBlackSpeed = 1f, lerpFadeAlpha, lerpMod;
        [SerializeField] private Image fadeToBlack, fadeToBlack2;
        [DoNotSerialize] public bool isGameStarting, doLerp;
        [SerializeField] private GameObject menuContainer;

        [SerializeField] private AnimateShuttle animateShuttle;
        [SerializeField] private AudioClip startResumeClick, exitClick;

        [SerializeField] private GameObject settingsMenu;
        private AudioSource audioSource;

        private bool gameOver;
        private bool hasPressedStart;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            fadeToBlack.enabled = false;
        }

        private void Update()
        {
            var checkState = GameStateMachine.Instance.GetState(GameStateName.GameMenuState);
            var isMainMenuState = GameStateMachine.Instance.IsCurrentState(checkState);
            checkState = GameStateMachine.Instance.GetState(GameStateName.GameOverState);
            var isGameOverState = GameStateMachine.Instance.IsCurrentState(checkState);
            if (!isMainMenuState && !isGameOverState) return;
            LerpFadeToBlack();
            if (lerpFadeAlpha >= 1 && doLerp)
            {
                doLerp = false;

                if (!isGameStarting)
                {
                    isGameStarting = true;
                    if (gameOver) return;
                    StartCoroutine(FadeToIntro());
                }
                else
                {
                    GameStateMachine.Instance.SetState(GameStateMachine.Instance.gameIntroState);
                    fadeToBlack.enabled = false;
                }
            }
        }


        public void PlayStartResumeClickAudio()
        {
            if (doLerp) return;
            audioSource.clip = startResumeClick;
            audioSource.Play();
        }

        public void OnSettingsButton()
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }

        public void PlayExitClickAudio()
        {
            audioSource.clip = exitClick;
            audioSource.Play();
        }

        private IEnumerator FadeToIntro()
        {
            lerpFadeAlpha = 0;
            menuContainer.gameObject.SetActive(false);
            GameStateMachine.Instance.playerController.cameraToControl.enabled = true;
            mainMenuCamera.enabled = false;
            yield return new WaitForSeconds(2f);
            lerpMod = 1;
            doLerp = true;
        }

        public void StartGame(bool done)
        {
            if (hasPressedStart) return;
            hasPressedStart = true;
            animateShuttle.Play();
            gameOver = done;
            if (done) fadeToBlack = fadeToBlack2;
            StartCoroutine(ReEnableStartButton());
            StartCoroutine(StartFadeToBlackDelayed(isGameStarting));
        }

        private IEnumerator ReEnableStartButton()
        {
            yield return new WaitForSeconds(5); //TODO fix
            hasPressedStart = false;
        }

        private IEnumerator StartFadeToBlackDelayed(bool skipWait = false)
        {
            if (!skipWait) yield return new WaitForSeconds(1); //TODO fix
            fadeToBlack.enabled = true;
            lerpMod = 0;
            isGameStarting = false;
            GameStateMachine.Instance.SetCursorLockMode(CursorLockMode.Locked);
            lerpFadeAlpha = 0;
            doLerp = true;
        }


        private void LerpFadeToBlack()
        {
            if (!doLerp) return;
            if (lerpFadeAlpha <= 1)
            {
                lerpFadeAlpha += Time.deltaTime * fadeToBlackSpeed;
                var colorAlpha = lerpMod == 0 ? lerpFadeAlpha : lerpMod - lerpFadeAlpha;
                fadeToBlack.color = new Color(0, 0, 0, colorAlpha);
            }
        }
    }
}