using Controllers;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTriggerDrill : DialogueTrigger
    {
        protected override void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            if (playerController.hasPickedDrill) return;
            base.OnTriggerExit(other);
        }

        protected override void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            playerController = iP.GetPlayerController();
            if (playerController.hasPickedDrill) return;
            base.OnTriggerStay(other);
        }
    }
}