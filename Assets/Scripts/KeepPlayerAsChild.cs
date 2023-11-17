using Controllers;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeepPlayerAsChild : MonoBehaviour
{
    public Transform playerRelPos;
    private bool hasShot;
    private Collider mCollider;

    private void Start()
    {
        mCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasShot) return;
        if (!other.TryGetComponent(out PlayerController pC)) return;
        var pCTransform = pC.gameObject.transform;
        pCTransform.parent = transform;
        pCTransform.position = playerRelPos.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController pC)) return;
        pC.gameObject.transform.parent = null;
        hasShot = true;
    }
}