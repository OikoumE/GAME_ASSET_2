using Controllers;
using StateMachine;
using UnityEngine;

//v. 0.3
namespace Interactables
{
    public class OpenDoor : MonoBehaviour
    {
        public enum DoorPosition
        {
            middle,
            left,
            right
        }

        public DoorPosition doorPosition;
        public bool canBeOpened;
        public float openSpeed = 10;
        [SerializeField] [Range(0, 100)] private float allowThroughDoorThreshold;
        [SerializeField] private BoxCollider mCollider, triggerBox;

        private float blendShapeAlpha;
        private bool isOpen;
        private SkinnedMeshRenderer mSkinnedMeshRenderer;

        private bool playerInRange;

        protected GameStateName state;

        public float AllowThroughDoorThreshold
        {
            set => allowThroughDoorThreshold = value;
        }

        public float OpenSpeed
        {
            get => openSpeed;
            set => openSpeed = value;
        }

        private bool Animate { get; set; }

        protected void Start()
        {
            mSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        protected void Update()
        {
            if (!Animate || !canBeOpened) return;
            if (!isOpen) // 0 == closed
            {
                blendShapeAlpha += Time.deltaTime * openSpeed;
                if (blendShapeAlpha >= allowThroughDoorThreshold)
                    mCollider.enabled = false;
                if (blendShapeAlpha >= 100)
                {
                    isOpen = true;
                    Animate = false;
                }
            }
            else if (isOpen) // 100 == open
            {
                blendShapeAlpha -= Time.deltaTime * openSpeed;
                if (blendShapeAlpha <= 0)
                {
                    isOpen = false;
                    Animate = false;
                }

                if (blendShapeAlpha <= allowThroughDoorThreshold)
                    mCollider.enabled = true;
            }

            SetBlendShape(blendShapeAlpha);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            var pC = iP.GetPlayerController();
            // if (!pC.hasPickedFuse || !pC.hasReadShuttleTablet) return;
            state = GameStateMachine.Instance.currentStateName;
            if (state is GameStateName.WireGameState or GameStateName.GameIntroState) return;
            if (canBeOpened)
                OpenAnimation();
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            var pC = iP.GetPlayerController();
            // if (!pC.hasPickedFuse || !pC.hasReadShuttleTablet) return;
            if (state is GameStateName.WireGameState or GameStateName.GameIntroState) return;
            if (canBeOpened)
                CloseAnimation();
        }


        public void OpenAnimation()
        {
            if (Animate) isOpen = !isOpen;
            Animate = true;
        }

        public void CloseAnimation()
        {
            if (Animate)
                isOpen = true;
            else
                Animate = true;
            mCollider.enabled = true;
        }


        public void SetTriggerBoxActive(bool enable)
        {
            triggerBox.enabled = enable;
        }

        public void SetBlendShape(float value)
        {
            mSkinnedMeshRenderer.SetBlendShapeWeight(0, value);
        }


        public float GetAlpha()
        {
            return blendShapeAlpha;
        }
    }
}