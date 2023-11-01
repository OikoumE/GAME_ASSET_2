using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoltAnimator : MonoBehaviour
{
    public enum BoltLocation
    {
        LeftTop,
        LeftBot,
        RightTop,
        RightBot
    }

    public BoltLocation boltLocation;
    [SerializeField] private Vector3 startPos, endPos;
    public float unScrewRotateSpeed, unScrewSpeed, impulseForce = 0.01f;
    private BoltController boltController;

    private float currAngle;
    private bool doAnimation;
    private bool hasGotPositions;
    private float lerpAlpha;
    private Rigidbody mRigidbody;
    private Quaternion startRotation;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.useGravity = false;
        mRigidbody.angularDrag = 5f;
        mRigidbody.drag = 5f;
        SetPositions();
    }

    private void Update()
    {
        if (!hasGotPositions || !doAnimation) return;
        var tr = transform;
        tr.position = Vector3.Lerp(startPos, endPos, lerpAlpha);

        transform.RotateAround(tr.position, tr.forward, unScrewRotateSpeed);

        if (lerpAlpha <= 1)
        {
            lerpAlpha += Time.deltaTime * unScrewSpeed;
            return;
        }

        mRigidbody.useGravity = true;
        mRigidbody.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);
        doAnimation = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(startPos, .005f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPos, .005f);
    }


    public void Interact()
    {
        doAnimation = true;
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
        startRotation = tr.rotation;
        SetStartEndPos(boltController.unScrewDistance);
        unScrewSpeed = boltController.unScrewSpeed;
        unScrewRotateSpeed = boltController.unScrewRotationSpeed;
        hasGotPositions = true;
    }
}