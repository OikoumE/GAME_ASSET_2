using System;
using Controllers;

namespace Interactables
{
    public class InteractableDrill : Interactable
    {
        private void Start()
        {
        }

        protected override void Update()
        {
        }

        public override void Interact(PlayerController pC)
        {
            if (pC.hasPickedDrill) return;
            pC.hasPickedDrill = true;
            gameObject.SetActive(false);
        }

        public override void Interact(KitchenDoorController kDC)
        {
            throw new NotImplementedException();
        }

        public override void Interact(FuseboxController fC)
        {
            throw new NotImplementedException();
        }
    }
}