using UnityEngine;

namespace Interactables
{
    public class FuseAnimator : MonoBehaviour
    {
        [SerializeField] private ParticleSystem mParticleSystem;
        [SerializeField] private float lerpSpeed;
        [SerializeField] private bool deactivateWhenDone;
        [SerializeField] private Vector3 lerpDistance;
        public float lerpAlpha;
        private bool animate;
        private Vector3 startPos, stopPos;

        public bool Done { get; private set; }


        private void Start()
        {
            SetStartStopPos();
        }

        private void Update()
        {
            if (!animate) return;
            if (lerpAlpha <= 1)
            {
                lerpAlpha += Time.deltaTime * lerpSpeed;
                transform.position = Vector3.Lerp(startPos, stopPos, lerpAlpha);
                return;
            }

            animate = false;
            Done = true;
            if (deactivateWhenDone)
                gameObject.SetActive(false);
            else mParticleSystem.Play();
        }
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            var gizSize = 0.01f;
            var gizmoSize = new Vector3(gizSize, gizSize, gizSize);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(startPos, gizmoSize);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPos, stopPos);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(stopPos, gizmoSize);
        }
#endif

        private void OnValidate()
        {
            SetStartStopPos();
        }

        private void SetStartStopPos()
        {
            var position = transform.position;
            startPos = position;
            stopPos = position + lerpDistance;
        }

        public void StartAnimation()
        {
            animate = true;
            if (deactivateWhenDone)
                mParticleSystem.Stop();
        }
    }
}