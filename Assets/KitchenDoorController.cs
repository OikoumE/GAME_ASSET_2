using UnityEngine;

public class KitchenDoorController : Interactable
{
    [SerializeField] private AudioSourceSettings lerpCamSetting;

    public bool animateDoor;
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


        // TODO deal with retracting control of camera,
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
        Debug.Log(hit.transform.gameObject.name);
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
        Debug.Log(doorRotatePoint.transform.rotation.y);
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