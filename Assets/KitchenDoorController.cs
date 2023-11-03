using UnityEngine;

public class KitchenDoorController : Interactable
{
    [SerializeField] private AudioSourceSettings lerpCamSetting;
    [HideInInspector] public bool animateDoor;
    [SerializeField] private Transform endPos, doorRotatePoint;
    [SerializeField] private float doorTranslateSpeed = 1, doorRotateSpeed = 1;

    private BoxCollider boxCollider;
    private CameraController cameraController;
    private bool cameraHasControl;
    private float doorTranslateLerpAlpha;
    private Vector3 startPos;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        startPos = doorRotatePoint.position;
        SetCamController();
    }

    protected override void Update()
    {
        base.Update();


        // todo prevent for interacting with panel without drill
        //TODO interact with wire puzzle game.....
        // - make targets clickable and drag-able
        // Vector3.Distance from IKTarget -> TargetLocation
        // if dist <= threshold, lerp from pos => target
        // play sound when threshold is met
        // puzzle-game need to inform kDC that game is over, ReturnToPlayer() 
        // TODO deal with retracting control of camera,
        // todo play ambiance electric sparky spark sounds to draw players attention to cabinet
        // todo incorporate drill, drill mechanics, drill animations, sounds
        if (cameraHasControl)
            cameraController.RunInputHandler();
        LerpToCam(lerpCamSetting);
        // if we dont add anything here... yeet eet
        DoRay();
        DoDoorAnimation();
    }

    private void OnDrawGizmos()
    {
        if (!cameraController) return;
        var camTransform = cameraController.cameraToControl.transform;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(camTransform.position, camTransform.forward * 10);
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
        if (!Input.GetMouseButtonDown(0) || !cameraHasControl) return;
        var ray = cameraController.cameraToControl.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, cameraController.interactableLayerMask)) return;
        if (hit.transform.gameObject.TryGetComponent(out IInteractable interactableObject))
            interactableObject.Interact(this);
    }

    private void DoDoorAnimation()
    {
        if (!animateDoor) return;
        if (doorTranslateLerpAlpha < 1)
            doorTranslateLerpAlpha += Time.deltaTime * doorTranslateSpeed;
        doorRotatePoint.transform.position = Vector3.Lerp(startPos, endPos.position, doorTranslateLerpAlpha);
        if (doorTranslateLerpAlpha <= 1) return;
        doorRotatePoint.transform.Rotate(Vector3.up, -doorRotateSpeed, Space.Self);
        if (doorRotatePoint.transform.rotation.y <= 0.66f) // magic number, trust me bro!
            animateDoor = false;
    }

    protected override void LerpToCam(AudioSourceSettings audioSourceSettings, bool interruptAudio = false)
    {
        if (!doLerp) return;
        if (audioSourceSettings.Source.clip != audioSourceSettings.audioClip || interruptAudio)
            PlayAudio(audioSourceSettings);
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
            cameraHasControl = true;
            InteractModeEnabled = !InteractModeEnabled;
        }
    }
}