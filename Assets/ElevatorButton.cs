using System;
using UnityEngine;

public class ElevatorButton : MonoBehaviour, I_Interactible
{
    public Camera playerCam, lerpCam, buttonCam1, buttonCam2;
    public int interactibleID;

    [SerializeField] private bool doLerp;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] [Range(0, 1)] private float lerpAlpha;
    private Camera fromCam;
    private bool interactModeEnabled;

    private PlayerController pC;
    private Camera targetCam;
    private Camera toCam;
    private GameObject toCamObject;
    private Vector3 toCamObjectPosition;
    private Quaternion toCamObjectRotation;
    private Transform toCamObjectTransform;


    private void Update()
    {
        if (!doLerp || !pC) return;
        if (lerpAlpha < 1)
            lerpAlpha += Time.deltaTime * lerpSpeed;
        LerpToCam();
    }


    public void Interact(int camID, PlayerController _pC)
    {
        _pC.SetPlayerControl(false);

        #region Setting From/To cam

        pC = _pC;
        fromCam = _pC.playerCam;
        switch (camID)
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


        if (interactModeEnabled)
        {
            fromCam = toCam;
            toCamObject = playerCam.gameObject;
            toCam = playerCam;
        }

        #endregion


        toCamObjectTransform = toCamObject.transform;
        toCamObjectPosition = toCamObjectTransform.position;
        toCamObjectRotation = toCamObjectTransform.rotation;


        //deactivate playercam, activate lerpcam and enable lerp
        doLerp = true;
    }

    private void LerpToCam()
    {
        if (!doLerp) return;
        // lerp lerpcam to InteractCam
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
            if (interactModeEnabled) pC.SetPlayerControl(true);
            interactModeEnabled = !interactModeEnabled;
        }
    }
}