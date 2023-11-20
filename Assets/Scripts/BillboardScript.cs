using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    public Transform cameraToFace;

    private void Start()
    {
        if (cameraToFace) return;
        cameraToFace = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        transform.LookAt(transform.position + cameraToFace.forward);
    }
}