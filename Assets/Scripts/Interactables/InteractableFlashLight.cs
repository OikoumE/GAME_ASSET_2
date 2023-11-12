using System;
using Controllers;
using Elevator;
using UnityEngine;

namespace Interactables
{
    public class InteractableFlashLight : Interactable
    {
        private void Start()
        {
        }

        protected override void Update()
        {
        }

        public override void Interact(
            PlayerController pC,
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio = true
        )
        {
            if (pC.hasFlashLight)
            {
                Debug.Log("hasFlashLight" + pC.hasFlashLight);
                return;
            }

            Debug.Log("hasFlashLight" + pC.hasFlashLight);

            pC.hasFlashLight = true;
            gameObject.SetActive(false);
            Debug.Log("hasFlashLight" + pC.hasFlashLight);
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