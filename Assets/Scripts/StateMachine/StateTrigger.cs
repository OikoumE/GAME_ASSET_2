using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [RequireComponent(typeof(Collider))]
    public class StateTrigger : MonoBehaviour
    {
        [SerializeField] private GameStateName stateToTrigger, specificState;
        [SerializeField] private bool requireSpecificState, triggerOnEnter, triggerOnExit;
        [SerializeField] private Vector3 triggerSize = Vector3.one;

        private BoxCollider mCollider;

        private void Awake()
        {
            SetColliderAsValues();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var pos = transform.position;
            var anyState = requireSpecificState ? specificState.ToString() : "Any";
            Handles.Label(pos + Vector3.up * (triggerSize.y / 2),
                "StateTrigger: " + anyState + " => " + stateToTrigger);
            Gizmos.DrawWireCube(pos, triggerSize);
        }
#endif
        private void OnTriggerEnter(Collider other)
        {
            if (!triggerOnEnter) return;
            if (requireSpecificState)
            {
                var checkState = GameStateMachine.Instance.GetState(specificState);
                var isCurrentState = GameStateMachine.Instance.IsCurrentState(checkState);
                if (!isCurrentState) return;
            }

            var newState = GameStateMachine.Instance.GetState(stateToTrigger);
            GameStateMachine.Instance.SetState(newState);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!triggerOnExit) return;
            if (requireSpecificState)
            {
                var checkState = GameStateMachine.Instance.GetState(specificState);
                var isCurrentState = GameStateMachine.Instance.IsCurrentState(checkState);
                if (!isCurrentState) return;
            }

            var newState = GameStateMachine.Instance.GetState(stateToTrigger);
            GameStateMachine.Instance.SetState(newState);
        }

        private void OnValidate()
        {
            SetColliderAsValues();
        }


        private void SetColliderAsValues()
        {
            if (!mCollider) mCollider = GetComponent<BoxCollider>();
            mCollider.isTrigger = true;
            mCollider.size = triggerSize;
        }
    }
}