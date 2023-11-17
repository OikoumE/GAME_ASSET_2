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

        [SerializeField] private Animation mAnimation;
        [SerializeField] private AudioClip startResumeClick, exitClick;
        private AudioSource audioSource;
        private bool gameOver;
        private AnimationClip shuttleIntro, shuttleOutro;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            fadeToBlack.enabled = false;
            shuttleIntro = mAnimation.GetClip("shuttleIntro");
            shuttleOutro = mAnimation.GetClip("shuttleOutro");
        }

        private void Update()
        {
            var isMainMenuState = GameStateMachine.Instance.IsCurrentState(GameStateName.GameMenuState);
            var isGameOverState = GameStateMachine.Instance.IsCurrentState(GameStateName.GameOverState);
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
            audioSource.clip = startResumeClick;
            audioSource.Play();
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
            mAnimation.Play();
            gameOver = done;
            if (done) fadeToBlack = fadeToBlack2;

            StartCoroutine(StartFadeToBlackDelayed(isGameStarting));
        }

        private IEnumerator StartFadeToBlackDelayed(bool skipWait = false)
        {
            if (!skipWait) yield return new WaitForSeconds(mAnimation.clip.length);
            fadeToBlack.enabled = true;
            lerpMod = 0;
            isGameStarting = false;
            GameStateMachine.Instance.SetCursorLockMode(CursorLockMode.Locked);
            lerpFadeAlpha = 0;
            doLerp = true;
            mAnimation.clip = shuttleOutro;
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