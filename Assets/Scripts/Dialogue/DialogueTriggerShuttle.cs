using Controllers;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTriggerShuttle : DialogueTrigger
    {
        protected override void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            playerController = iP.GetPlayerController();
            if (playerController.hasPickedFuse && playerController.hasReadShuttleTablet) return;
            base.OnTriggerStay(other);
        }
    }
}