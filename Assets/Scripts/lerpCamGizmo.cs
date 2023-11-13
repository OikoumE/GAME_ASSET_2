using UnityEngine;

public class LerpCamGizmo : MonoBehaviour
{
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
#endif
}