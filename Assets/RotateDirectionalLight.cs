using UnityEngine;

public class RotateDirectionalLight : MonoBehaviour
{
    public float rotateSpeed = 10f;
    private float yRot;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed);
    }
}