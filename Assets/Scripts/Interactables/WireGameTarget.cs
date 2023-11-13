using System;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables
{
    public class WireGameTarget : Interactable
    {
        [SerializeField] private Material ledIndicatorMaterial;
        [SerializeField] private MeshRenderer ledIndicator;
        [SerializeField] private float ledIntensity;
        [SerializeField] private Transform targetLocation;
        [Range(0, .1f)] [SerializeField] private float targetDistanceThreshold = 0.075f;

        [FormerlySerializedAs("particleSystem")] [SerializeField]
        private ParticleSystem mParticleSystem;

        [SerializeField] private Transform targetConstraintTransform;

        private BoxCollider boxCollider;
        private bool hasInteractedWithTarget;

        private bool hasReachedTarget;
        private KitchenDoorController mKDc;

        private Vector3 mousePos;

        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            if (!targetLocation) throw new Exception("No target set for wire: " + gameObject.name);
        }

        protected override void Update()
        {
            if (!mKDc || !mKDc.doorIsOpen) return;
            base.Update(); //!!keep!! calculates lerp 
            if (!hasInteractedWithTarget || hasReachedTarget) return;
            mousePos = mKDc.hit.point;
            if (Input.GetMouseButton(0))
            {
                var position = transform.position;
                transform.position = mousePos;


                var dst = Vector3.Distance(position, targetLocation.position);
                if (dst >= targetDistanceThreshold) return;
                // we have reached target
                transform.position = targetLocation.position;
                hasReachedTarget = true;
                mKDc.numberOfConnectedWires++;
                boxCollider.enabled = false;
                if (mParticleSystem) mParticleSystem.Stop();
                mKDc.PlayWireConnectedAudio();
                ledIndicator.material = ledIndicatorMaterial;
            }
            else
            {
                if (hasReachedTarget) return;
                hasInteractedWithTarget = false;
                boxCollider.enabled = true;

                // TODO
                // snap target back to bonePos on release
                transform.position = targetConstraintTransform.position;
            }
        }
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!hasInteractedWithTarget || !targetLocation) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetLocation.position, targetDistanceThreshold);
        }

#endif

        public override void Interact(KitchenDoorController kDc)
        {
            //kDc is where we are at
            boxCollider.enabled = false;
            mKDc = kDc;
            hasInteractedWithTarget = true;
        }

        public override void Interact(FuseboxController fC)
        {
            throw new NotImplementedException();
        }
    }
}