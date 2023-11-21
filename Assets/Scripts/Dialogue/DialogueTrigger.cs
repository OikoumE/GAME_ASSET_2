using Controllers;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : DialogueController
    {
        public bool useOnExit;


        protected override void OnTriggerExit(Collider other)
        {
            if (!useOnExit) return;
            base.OnTriggerExit(other);
        }

        protected override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);
        }

        public void DuctTapeSolution()
        {
            SetDialogueText();
            SetDialogueVisibility(true);
        }
    }
}