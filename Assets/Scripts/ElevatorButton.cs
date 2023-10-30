using System;
using System.Collections;
using UnityEngine;

public enum Floor
{
    Engine,
    Restaurant,
    Kitchen
}

public class ElevatorButton : MonoBehaviour, I_Interactible
{
    public Camera playerCam, lerpCam, buttonCam1, buttonCam2;

    [SerializeField] private Texture2D greenHand, redHand;
    [SerializeField] private float lerpSpeed = 1f;

    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private BoxCollider[] boxColliders;


    private bool doLerp;
    private ElevatorController elevatorController;
    private Camera fromCam;
    private Vector3 hitPoint;
    private int lastPanel;
    [Range(0, 1)] private float lerpAlpha;

    private PlayerController pC;
    private Camera toCam;
    private GameObject toCamObject;
    private Vector3 toCamObjectPosition;
    private Quaternion toCamObjectRotation;
    private Transform toCamObjectTransform;

    public bool InteractModeEnabled { get; private set; }


    private void Start()
    {
        boxColliders = GetComponentsInChildren<BoxCollider>();
        elevatorController = GetComponentInParent<ElevatorController>();
    }

    private void Update()
    {
        DoRay();
        if (!doLerp || !pC) return;
        LerpToCam();
    }

    private void OnDrawGizmos()
    {
        if (!toCamObject) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(toCamObject.transform.position, hitPoint);
    }

    public void Interact(int panelId, PlayerController _pC)
    {
        _pC.SetPlayerControl(false);

        #region Setting From/To cam

        lastPanel = panelId;
        pC = _pC;
        fromCam = _pC.playerCam;
        switch (panelId)
        {
            case 1:
                toCamObject = buttonCam1.gameObject;
                toCam = buttonCam1;
                break;
            case 2:
                toCamObject = buttonCam2.gameObject;
                toCam = buttonCam2;
                break;
            default:
                throw new NotImplementedException();
        }

        if (InteractModeEnabled)
        {
            fromCam = toCam;
            toCamObject = playerCam.gameObject;
            toCam = playerCam;
        }

        #endregion

        toCamObjectTransform = toCamObject.transform;
        toCamObjectPosition = toCamObjectTransform.position;
        toCamObjectRotation = toCamObjectTransform.rotation;

        doLerp = true; // trigger lerp
    }

    private void SetColliderState(bool state)
    {
        foreach (var box in boxColliders) box.enabled = state;
    }

    private void PlayAudio(AudioSourceSettings settings, bool interrupt = true)
    {
        var audioSource = settings.Source;
        if (interrupt) audioSource.Stop();
        audioSource.pitch = settings.pitch;
        audioSource.volume = settings.volume;
        audioSource.clip = settings.audioClip;
        audioSource.Play();
    }

    private void DoRay()
    {
        if (!InteractModeEnabled) return;
        var activeFloor = elevatorController.ActiveFloor;
        var ray = toCam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, 1000)) return;
        hitPoint = hit.point;
        var objectHit = hit.transform;

        var isElevatorButtonHitBox = objectHit.gameObject.TryGetComponent(out ElevatorButtonHitBox eButtonHitBox);
        if (!isElevatorButtonHitBox)
        {
            Cursor.SetCursor(redHand, Vector2.zero, CursorMode.Auto);
            return;
        }

        var handColor = activeFloor == eButtonHitBox.floor ? redHand : greenHand;
        Cursor.SetCursor(handColor, Vector2.zero, CursorMode.Auto);

        if (!Input.GetMouseButtonDown(0))
            return; // mouse is not clicked or we are hovering activeFloor;

        if (activeFloor == eButtonHitBox.floor)
        {
            PlayAudio(audioSettings.errorSettings);
            return;
        }

        PlayAudio(audioSettings.clickSettings);
        // button has been clicked and cursor is not over activeFloor;
        elevatorController.ActiveFloor = eButtonHitBox.floor; // set active floor
        StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        pC.SetCursorLockMode(CursorLockMode.Locked); // toggle cursor off/lock mouse
        var waitAmount = elevatorController.runtimeValues.returnToPlayerCooldown;
        yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
        Interact(lastPanel, pC); // trigger lerp back to player
        yield return new WaitForSeconds(waitAmount); // cooldown before returning to playerCam.
        StartCoroutine(elevatorController.MoveElevatorToFloor()); // close doors.
    }


    private void LerpToCam()
    {
        if (!doLerp) return;
        if (lerpAlpha < 1)
            lerpAlpha += Time.deltaTime * lerpSpeed;
        if (!(audioSettings.lerpSettings.Source.clip == audioSettings.lerpSettings.audioClip))
            PlayAudio(audioSettings.lerpSettings);
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

        // when lerp is 1 or more, set "interactCam" active, deactivate lerpcam
        if (lerpAlpha >= 1)
        {
            lerpCam.enabled = false;
            toCam.enabled = true;
            lerpAlpha = 0;
            doLerp = false;
            if (InteractModeEnabled) pC.SetPlayerControl(true);
            else pC.SetCursorLockMode(CursorLockMode.None); // toggle cursor on / unlock mouse
            SetColliderState(InteractModeEnabled);
            InteractModeEnabled = !InteractModeEnabled;
        }
    }

    [Serializable]
    private class AudioSettings
    {
        [SerializeField] public AudioSourceSettings errorSettings;
        [SerializeField] public AudioSourceSettings clickSettings;
        [SerializeField] public AudioSourceSettings lerpSettings;
        [SerializeField] public AudioClip lerpCamAudioClip;
        [SerializeField] public AudioClip buttonClickAudioClip;
        [SerializeField] public AudioClip buttonErrorAudioClip;
    }
}