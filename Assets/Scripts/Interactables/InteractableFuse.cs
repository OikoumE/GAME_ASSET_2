using System;
using Controllers;
using HighlightPlus;
using StateMachine;
using UnityEngine;

namespace Interactables
{
    public class InteractableFuse : Interactable
    {
        [SerializeField] private HighlightEffect highlightEffect;
        [SerializeField] private HighlightTrigger highlightTrigger;
        [SerializeField] private GameObject imageObj;
        private InteractableFuse[] fuses;

        private void Start()
        {
            fuses = FindObjectsOfType<InteractableFuse>();
            if (!highlightEffect) highlightEffect = GetComponentInParent<HighlightEffect>();
            if (!highlightTrigger) highlightTrigger = GetComponentInParent<HighlightTrigger>();
        }


        public void SetHighlightEnabled(bool enable)
        {
            highlightEffect.enabled = enable;
            highlightTrigger.enabled = enable;
        }

        public override void Interact(
            PlayerController pC,
            AudioSourceSettings audioSourceSettings,
            bool interruptAudio = true
        )
        {
            if (pC.hasPickedFuse) return;
            pC.hasPickedFuse = true;
            foreach (var interactableFuse in fuses)
            {
                if (interactableFuse == this) continue;
                interactableFuse.isInteractable = false;
                interactableFuse.SetHighlightEnabled(false);
            }

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