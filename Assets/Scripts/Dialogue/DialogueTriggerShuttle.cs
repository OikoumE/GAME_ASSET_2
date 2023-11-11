using Controllers;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTriggerShuttle : DialogueTrigger
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            playerController = iP.GetPlayerController();
            if (playerController.hasPickedFuse && playerController.hasReadShuttleTablet) return;
            base.OnTriggerEnter(other);
        }
    }
}