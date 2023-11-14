using Controllers;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : DialogueController
    {
        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
        }

        protected override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);
        }
    }
}