using Controllers;
using UnityEngine;

namespace Interactables
{
    public class FuseBoxDoorKnob : Interactable
    {
        [SerializeField] private float doorKnobSpeed = 1f;
        private bool animateDoorKnob;
        private BoxCollider boxCollider;

        private FuseboxController fuseboxController;


        private float rotAlpha;

        // Start is called before the first frame update
        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            AnimateDoorKnob();
        }

        private void AnimateDoorKnob()
        {
            if (!animateDoorKnob) return;
            transform.localRotation = Quaternion.Euler(0, rotAlpha, 0);
            if (rotAlpha <= 90)
            {
                rotAlpha += doorKnobSpeed;
                return;
            }

            fuseboxController.animateDoor = true;
            animateDoorKnob = false;
        }


        public override void Interact(FuseboxController fC)
        {
            fuseboxController = fC;
            animateDoorKnob = true;
            isInteractable = false;
            boxCollider.enabled = false;
        }
    }
}