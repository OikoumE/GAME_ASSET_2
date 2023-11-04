using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WireGameTarget : Interactable
{
    [SerializeField] private Transform targetLocation;
    [Range(0, .1f)] [SerializeField] private float targetDistanceThreshold = 0.075f;

    [FormerlySerializedAs("particleSystem")] [SerializeField]
    private ParticleSystem mParticleSystem;

    private BoxCollider boxCollider;
    private bool hasInteractedWithTarget;

    private bool hasReachedTarget;
    private KitchenDoorController mKDc;

    private Vector3 mousePos;


    // Start is called before the first frame update
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (!targetLocation) throw new Exception("No target set for wire: " + gameObject.name);
    }

    protected override void Update()
    {
        base.Update(); //!!keep!! calculates lerp 
        if (!hasInteractedWithTarget || hasReachedTarget) return;
        mousePos = mKDc.hit.point;
        if (Input.GetMouseButton(0))
        {
            transform.position = mousePos;
            var dst = Vector3.Distance(transform.position, targetLocation.position);
            if (dst >= targetDistanceThreshold) return;
            // we have reached target
            transform.position = targetLocation.position;
            hasReachedTarget = true;
            mKDc.numberOfConnectedWires++;
            boxCollider.enabled = false;
            if (mParticleSystem) mParticleSystem.Stop();
            //todo make sparks.
            //todo make sound.
            //todo record amount of completed wires.
        }
        else
        {
            if (hasReachedTarget) return;
            hasInteractedWithTarget = false;
            boxCollider.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (!hasInteractedWithTarget || !targetLocation) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetLocation.position, .01f);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(targetLocation.position, targetDistanceThreshold);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, .01f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(targetLocation.position, transform.position);
    }


    public override void Interact(KitchenDoorController kDc)
    {
        //kDc is where we are at
        boxCollider.enabled = false;
        mKDc = kDc;
        hasInteractedWithTarget = true;
    }
}