using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public MeshRenderer kitchenButton, restaurantButton, engineButton;
    public Material selectedButton, inactiveButton;
    public GameObject door1, door2;

    public Camera playerCam, lerpCam, cam1, cam2;
    //TODO depending on which panel the player interacts with,
    //TODO we move LERPCAM to PLAYERCAM pos, swap to LERPCAM,
    //TODO then lerp LERPCAM to either CAM1 or CAM2


    public float openSpeed;

    public bool playerHasInteracted;
    private readonly Quaternion door1EndPos = new(-0.61237f, -0.35355f, -0.35355f, 0.61237f);
    private readonly Quaternion door2EndPos = new(-0.35355f, -0.61237f, -0.61237f, 0.35355f);
    private readonly Quaternion startPos = new(-0.50f, -0.5f, -0.5f, 0.5f);
    private float alpha;

    private bool animateDoors;

    private bool isOpen;

    private void Start()
    {
    }

    private void Update()
    {
        AnimateDoors();

        //TODO need more checks for keypress.
        // hasSelectedFloor ??
        if (Input.GetKeyDown(KeyCode.E)) OnInteract();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        animateDoors = true;
        isOpen = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        animateDoors = true;
        isOpen = true;
    }

    private void OnInteract()
    {
        animateDoors = true;
        isOpen = true;
    }

    private void AnimateDoors()
    {
        if (!animateDoors) return;
        if (!isOpen)
            alpha += Time.deltaTime * openSpeed;
        else
            alpha -= Time.deltaTime * openSpeed;

        switch (alpha)
        {
            case <= 0:
                isOpen = false;
                animateDoors = false;
                break;
            case >= 1:
                isOpen = true;
                animateDoors = false;
                break;
        }

        door1.transform.rotation = Quaternion.Lerp(
            startPos, door1EndPos, alpha);
        door2.transform.rotation = Quaternion.Lerp(
            startPos, door2EndPos, alpha);
    }
}