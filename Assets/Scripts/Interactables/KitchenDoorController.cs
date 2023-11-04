using System;
using System.Collections;
using UnityEngine;

public class KitchenDoorController : Interactable, IInteractable
{
    [SerializeField] private AudioSourceSettings lerpCamSetting;
    public bool animateDoor;
    [SerializeField] private Transform endPos, doorRotatePoint;
    [SerializeField] private float doorTranslateSpeed = 1, doorRotateSpeed = 1;
    [SerializeField] private GameObject doorMesh;
    public int numberOfConnectedWires;

    [SerializeField] [Range(0, 10)] private float waitAfterComplete = 1f;

    private BoxCollider boxCollider;
    private CameraController cameraController;
    private bool cameraHasControl;
    private Rigidbody doorMeshRb;
    private float doorTranslateLerpAlpha;

    private bool hasReturnedToPlayer;
    [HideInInspector] public RaycastHit hit;
    private Vector3 startPos;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (!doorMesh) throw new Exception("Set doorMesh");
        doorMeshRb = doorMesh.GetComponent<Rigidbody>();
        doorMeshRb.isKinematic = true;
        doorMeshRb.useGravity = false;

        startPos = doorRotatePoint.position;
        SetCamController();
        if (animateDoor) throw new Exception("Remember to disable animateDoor");
    }

    protected override void Update()
    {
        base.Update();
        LerpToCam(lerpCamSetting);
        if (numberOfConnectedWires == 6 && !hasReturnedToPlayer)
        {
            // we have all wires connected!
            hasReturnedToPlayer = true;
            StartCoroutine(ReturnToPlayer());
        }
        //TODO play sound when target threshold is met


        // TODO deal with retracting control of camera,


        // todo incorporate drill, drill mechanics, drill animations, sounds
        // todo prevent for interacting with panel without drill
        // TODO interact with wire puzzle game.....
        // todo play ambiance electric sparky spark sounds to draw players attention to cabinet
        if (cameraHasControl)
            cameraController.RunInputHandler();
        DoRay();
        DoDoorAnimation();
    }


    private void OnValidate()
    {
        SetCamController();
    }

    private void SetCamController()
    {
        cameraController = fixedCam.gameObject.GetComponent<CameraController>();
    }

    private void DoRay()
    {
        var ray = cameraController.cameraToControl.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, cameraController.interactableLayerMask)) return;
        if (!Input.GetMouseButtonDown(0) || !cameraHasControl) return;
        if (hit.transform.gameObject.TryGetComponent(out IInteractable interactableObject))
            interactableObject.Interact(this);
    }

    protected override IEnumerator ReturnToPlayer()
    {
        playerController.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
        yield return new WaitForSeconds(waitAfterComplete); // cooldown before returning to playerCam.
        Interact(playerController); // trigger lerp back to player
    }

    private void DoDoorAnimation()
    {
        if (!animateDoor) return;
        if (doorTranslateLerpAlpha < 1)
            doorTranslateLerpAlpha += Time.deltaTime * doorTranslateSpeed;
        doorRotatePoint.transform.position = Vector3.Lerp(startPos, endPos.position, doorTranslateLerpAlpha);
        if (doorTranslateLerpAlpha <= 1) return;
        doorRotatePoint.transform.Rotate(Vector3.up, -doorRotateSpeed, Space.Self);
        if (doorRotatePoint.transform.rotation.y >= 0.69f) return; // magic number, trust me bro!
        doorMeshRb.isKinematic = false;
        doorMeshRb.useGravity = true;
        animateDoor = false;
    }

    protected override void LerpToCam(AudioSourceSettings audioSourceSettings, bool interruptAudio = false)
    {
        if (!doLerp) return;
        if (audioSourceSettings.Source.clip != audioSourceSettings.audioClip || interruptAudio)
            PlayAudio(audioSourceSettings);
        // lerp lerpCam to InteractCam
        lerpCam.transform.position = Vector3.Lerp(
            fromCam.transform.position,
            toCamObjectPosition,
            lerpAlpha
        );
        lerpCam.transform.rotation = Quaternion.Lerp(
            fromCam.transform.rotation,
            toCamObjectRotation,
            lerpAlpha);

        if (!lerpCam.enabled)
        {
            lerpCam.enabled = true;
            fromCam.enabled = false;
        }

        if (lerpAlpha >= 1)
        {
            lerpCam.enabled = false;
            toCam.enabled = true;
            lerpAlpha = 0;
            doLerp = false;
            if (InteractModeEnabled) playerController.SetPlayerControl(true);
            boxCollider.enabled = false;
            cameraHasControl = !InteractModeEnabled;
            InteractModeEnabled = !InteractModeEnabled;
        }
    }
}