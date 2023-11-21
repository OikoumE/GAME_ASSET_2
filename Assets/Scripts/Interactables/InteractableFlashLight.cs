using System;
using Controllers;
using StateMachine;
using UnityEngine;

namespace Interactables
{
    public class InteractableFlashLight : Interactable
    {
        [SerializeField] private GameObject imageObj;

        
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
            if (pC.hasFlashLight) return;
            pC.hasFlashLight = true;
            imageObj.SetActive(true);
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