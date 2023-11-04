using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoltAnimator : Interactable
{
    [SerializeField] private Vector3 startPos, endPos;
    public float unScrewRotateSpeed, impulseForce = 0.01f;
    private BoltController boltController;
    private float currAngle;

    private float drillAnimDuration;
    private bool hasGotPositions;
    private KitchenDoorController kitchenDoorController;
    private Rigidbody mRigidbody;
    private AudioSource unscrewAudioSource;

    private void Start()
    {
        unscrewAudioSource = GetComponent<AudioSource>();
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.useGravity = false;
        mRigidbody.isKinematic = true;
        mRigidbody.angularDrag = 5f;
        mRigidbody.drag = 5f;
        SetPositions();
    }

    protected override void Update()
    {
        base.Update();
        if (!hasGotPositions || !doLerp) return;
        var tr = transform;
        tr.position = Vector3.Lerp(startPos, endPos, lerpAlpha);

        transform.RotateAround(tr.position, tr.forward, -unScrewRotateSpeed);
        if (!unscrewAudioSource.isPlaying) unscrewAudioSource.Play();
        if (lerpAlpha <= 1) return;
        if (unscrewAudioSource.isPlaying) unscrewAudioSource.Stop();

        var screwCount = boltController.unScrewedScrews;
        if (screwCount < 4) boltController.unScrewedScrews++;
        if (boltController.unScrewedScrews == 4) kitchenDoorController.animateDoor = true;


        mRigidbody.isKinematic = false;
        mRigidbody.useGravity = true;
        mRigidbody.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);
        doLerp = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(startPos, .005f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPos, .005f);
    }

    public override void Interact(KitchenDoorController kDC)
    {
        kitchenDoorController = kDC;
        // doLerp = true;
        StartCoroutine(ActivateLerp());
    }

    private IEnumerator ActivateLerp()
    {
        yield return new WaitForSeconds(drillAnimDuration);
        doLerp = true;
    }

    public override void Interact(PlayerController pC)
    {
        //do nothing
        // SUCCESS!!!
    }

    public void SetStartEndPos(float unScrewDistance)
    {
        var tr = transform;
        startPos = tr.position;
        endPos = startPos - tr.forward * -unScrewDistance;
    }

    private void SetPositions()
    {
        if (boltController == null) boltController = GetComponentInParent<BoltController>();
        var tr = transform;
        SetStartEndPos(boltController.unScrewDistance);
        drillAnimDuration = boltController.drillAnimDuration;
        lerpSpeed = boltController.unScrewSpeed;
        unScrewRotateSpeed = boltController.unScrewRotationSpeed;
        hasGotPositions = true;
    }
}