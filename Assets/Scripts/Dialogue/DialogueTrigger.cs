using Controllers;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : DialogueController
    {
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
        }
    }
}