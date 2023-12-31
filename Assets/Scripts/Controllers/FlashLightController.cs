using System.Collections;
using StateMachine;
using UnityEngine;

namespace Controllers
{
    public class FlashLightController : MonoBehaviour
    {
        [SerializeField] private Material buttonEmitGreen, buttonEmitRed;
        [SerializeField] private Vector3 outPos;
        [SerializeField] private Transform parent, button;
        [SerializeField] private float lerpSpeed = 1;
        [SerializeField] private GameObject flashLightGo;
        private readonly Vector3 buttonOffRotation = new(90, 0, 0);
        private readonly Vector3 buttonOnRot = new(-90, 0, 0);

        private readonly Vector3 inPos = Vector3.zero;
        private bool animate;
        private MeshRenderer buttonMeshRenderer;
        private bool isIn;
        private float lerpAlpha = 1;
        private PlayerController playerController;
        private Light spotLight;
        private Vector3 startLerpPos, endLerpPos;

        private void Start()
        {
            playerController = GetComponentInParent<PlayerController>();
            buttonMeshRenderer = button.gameObject.GetComponent<MeshRenderer>();
            spotLight = GetComponentInChildren<Light>();

            Init();
        }

        private void Update()
        {
            if (!playerController.hasFlashLight) return;
            if (!GameStateMachine.Instance.playerController.playerHasControl) return;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isIn) StartCoroutine(StartAnimateOutDelayed());
                else AnimateIn();
            }

            Animate();
        }


        private void Init()
        {
            flashLightGo.SetActive(false);
            transform.localPosition = outPos;
            lerpAlpha = 0;
            isIn = false;
            SetButtonState(false);
        }

        public void SetFlashLightTurnedOn(bool enable)
        {
            if (enable)
            {
                if (!isIn) AnimateIn();
            }
            else
            {
                StartCoroutine(StartAnimateOutDelayed());
            }
        }


        private void Animate()
        {
            if (!animate) return;
            if (lerpAlpha <= 1) lerpAlpha += Time.deltaTime * lerpSpeed;
            transform.localPosition = Vector3.Lerp(startLerpPos, endLerpPos, lerpAlpha);


            if (lerpAlpha <= 1) return;
            if (isIn)
                SetButtonState(true);
            else
                flashLightGo.SetActive(false);
            animate = false;
        }

        private void SetButtonState(bool enable)
        {
            if (enable)
            {
                button.localRotation = Quaternion.Euler(buttonOnRot);
                spotLight.enabled = true;
                buttonMeshRenderer.material = buttonEmitGreen;
                return;
            }

            button.localRotation = Quaternion.Euler(buttonOffRotation);
            spotLight.enabled = false;
            buttonMeshRenderer.material = buttonEmitRed;
        }

        private void AnimateIn()
        {
            flashLightGo.SetActive(true);
            lerpAlpha = 0;
            animate = true;
            startLerpPos = outPos;
            endLerpPos = inPos;
            isIn = true;
        }


        private IEnumerator StartAnimateOutDelayed()
        {
            SetButtonState(false);
            yield return new WaitForSeconds(.5f);
            AnimateOut();
        }


        private void AnimateOut()
        {
            lerpAlpha = 0;
            animate = true;
            startLerpPos = inPos;
            endLerpPos = outPos;
            isIn = false;
        }
    }
}