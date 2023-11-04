using UnityEngine;

public class TempDoorScript : MonoBehaviour
{
    [Range(0, 360)] [SerializeField] private float rotAlpha;
    [SerializeField] private GameObject door;

    private void OnValidate()
    {
        door.transform.rotation = new Quaternion(0, rotAlpha, 0, 1);
    }
}