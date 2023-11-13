using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [RequireComponent(typeof(Collider))]
    public class StateTrigger : MonoBehaviour
    {
        [SerializeField] private GameStateName stateToTrigger, specificState;
        [SerializeField] private bool requireSpecificState, triggerOnEnter, triggerOnExit;


        private Collider mCollider;

        private void Awake()
        {
            SetColliderAsTrigger();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var pos = transform.position;
            Handles.Label(pos + Vector3.up * 0.5f, "StateTrigger: " + stateToTrigger);
            Gizmos.DrawSphere(pos, 0.1f);
        }
#endif
        private void OnTriggerEnter(Collider other)
        {
            if (!triggerOnEnter) return;
            if (requireSpecificState)
            {
                var isCurrentState = GameStateMachine.Instance.IsCurrentState(specificState);
                if (!isCurrentState) return;
            }

            GameStateMachine.Instance.SetState(stateToTrigger);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!triggerOnExit) return;
            if (requireSpecificState)
            {
                var isCurrentState = GameStateMachine.Instance.IsCurrentState(specificState);
                if (!isCurrentState) return;
            }

            GameStateMachine.Instance.SetState(stateToTrigger);
        }

        private void OnValidate()
        {
            SetColliderAsTrigger();
        }


        private void SetColliderAsTrigger()
        {
            if (!mCollider) mCollider = GetComponent<Collider>();
            mCollider.isTrigger = true;
        }
    }
}