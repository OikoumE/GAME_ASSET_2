using Controllers;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace WaypointSystem
{
    [RequireComponent(typeof(SphereCollider))]
    public class ChildMoveDetector : MonoBehaviour
    {
        private GizmoChildren gizmoChildren;
        private SphereCollider sphereCollider;

        private void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            gizmoChildren = GetComponentInParent<GizmoChildren>();
            sphereCollider.radius = gizmoChildren.triggerAreaSize;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, gameObject.name);
        }
#endif

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;

            gameObject.SetActive(false);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;

            gameObject.SetActive(false);
        }
    }
}