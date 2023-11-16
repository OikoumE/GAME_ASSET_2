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
        [SerializeField] private Image fadeToBlack;
        [DoNotSerialize] public bool isGameStarting, doLerp;
        [SerializeField] private GameObject menuContainer;

        private void Start()
        {
            fadeToBlack.enabled = false;
        }

        private void Update()
        {
            var isMainMenuState = GameStateMachine.Instance.IsCurrentState(GameStateName.GameMenuState);
            if (!isMainMenuState) return;
            LerpFadeToBlack();


            if (lerpFadeAlpha >= 1 && doLerp)
            {
                doLerp = false;

                if (!isGameStarting)
                {
                    isGameStarting = true;
                    StartCoroutine(FadeToIntro());
                }
                else
                {
                    GameStateMachine.Instance.SetState(GameStateMachine.Instance.gameIntroState);
                    fadeToBlack.enabled = false;
                }
            }
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

        public void StartGame()
        {
            fadeToBlack.enabled = true;
            GameStateMachine.Instance.SetCursorLockMode(CursorLockMode.Locked);
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